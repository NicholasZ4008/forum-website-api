using Microsoft.EntityFrameworkCore;

public class EmailDb : DbContext
{
    public EmailDb(DbContextOptions options) : base(options) { }

    public DbSet<Email> Emails { get; set; }
}