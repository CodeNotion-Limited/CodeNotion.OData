using Microsoft.EntityFrameworkCore;

namespace CodeNotion.Odata.Tests.NaviagtaionPropertiesTests;

public class NavigationPropertiesTestDbContext : DbContext
{
    public DbSet<NavigationPropertiesTestEntity> Entities => Set<NavigationPropertiesTestEntity>();
    public DbSet<NavigationPropertiesTestSubEntity> SubEntities => Set<NavigationPropertiesTestSubEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer();
    }
}