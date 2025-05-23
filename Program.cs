using Microsoft.EntityFrameworkCore;
using Npgsql;

DotNetEnv.Env.Load();

//CORS
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

//Builder BoilerPlate
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:3000",
                                            "https://localhost:3000",
                                            "http://theforumuniversity.com",
                                            "https://theforumuniversity.com",
                                            "http://www.theforumuniversity.com",
                                            "https://www.theforumuniversity.com",
                                            "http://api.theforumuniversity.com",
                                            "https://api.theforumuniversity.com"
                                              )
                                              .AllowAnyHeader()
                                              .AllowAnyMethod();
                                        
                      });
});


builder.Services.AddDbContext<EmailDb>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "ForumEmailAPI";
    config.Title = "EmailAPI v1";
    config.Version = "v1";
});

builder.Services.AddHealthChecks();

var app = builder.Build();

//allow specific origins
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

//preflight handlers:
// app.MapMethods("/emails", new[] { "OPTIONS" }, () => Results.Ok());
// app.MapMethods("/emails/{id}", new[] { "OPTIONS" }, () => Results.Ok());

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "ForumEmailAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

//automate db migrations for prod deployment
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<EmailDb>();
    db.Database.Migrate();
}

//coolify health check
app.MapHealthChecks("/health");

//CRUD Functions
app.MapGet("/emails", async(EmailDb db) =>
    await db.Emails.ToListAsync()
);

app.MapGet("/emails/{id}", async (int id, EmailDb db) =>
    await db.Emails.FindAsync(id)
        is Email email
            ? Results.Ok(email)
            : Results.NotFound()
);

app.MapPost("/emails", async (Email email, EmailDb db)=>{

    // Check if email already exists
    if (await db.Emails.AnyAsync(e => e.EmailName == email.EmailName))
        return Results.Conflict("Email already registered");
    
    db.Emails.Add(email);
    await db.SaveChangesAsync();

    return Results.Created($"/emails/{email.Id}", email);
});

app.MapPut("/emails/{id}", async (int id, Email inputEmail, EmailDb db) =>
{
    var email = await db.Emails.FindAsync(id); //find email based on the id

    if (email is null) return Results.NotFound();

    email.EmailName = inputEmail.EmailName;//replace in-db email based on input Email

    await db.SaveChangesAsync();

    return Results.NoContent(); //save the result
});

app.MapDelete("/emails/{id}", async (int id, EmailDb db) =>
{
    if (await db.Emails.FindAsync(id) is Email email)
    {
        db.Emails.Remove(email);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.MapGet("/test-connect", async (EmailDb db) =>
{
    var canConnect = await db.Database.CanConnectAsync();
    return canConnect
        ? Results.Ok("Connected successfully to PostgreSQL!")
        : Results.Problem("Cannot connect to PostgreSQL", statusCode: StatusCodes.Status503ServiceUnavailable);
});

app.MapGet("/export", async (EmailDb db, HttpResponse response) =>
{
    var emails = await db.Emails.ToListAsync();
    
    // Set response headers
    response.ContentType = "text/csv";
    response.Headers.Append("Content-Disposition", "attachment; filename=emails.csv");
    
    // Create CSV content - using async methods
    await response.StartAsync();
    
    // Write header
    await response.WriteAsync("Id,EmailName\n");
    
    // Write data rows - all using async methods
    foreach (var email in emails)
    {
        await response.WriteAsync($"{email.Id},{email.EmailName}\n");
    }
    
    await response.CompleteAsync();
});

app.Run();