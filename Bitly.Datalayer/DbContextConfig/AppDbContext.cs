using Microsoft.EntityFrameworkCore;

namespace Bitly.Database;

public class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("bitly");
    }

    public DbSet<ShortnedUrl> ShortnedUrls { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().HaveMaxLength(1024);
    }
}
