using Microsoft.EntityFrameworkCore;

class EmailDb : DbContext
{
    public EmailDb(DbContextOptions<EmailDb> options)
        : base(options) { }

    public DbSet<Email> Emails => Set<Email>();
}