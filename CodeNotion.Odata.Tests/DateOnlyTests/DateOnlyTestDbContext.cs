using Microsoft.EntityFrameworkCore;

namespace CodeNotion.Odata.Tests.DateOnlyTests;

public class DateOnlyTestDbContext : DbContext
{
    public DbSet<DateOnlyTestEntity> Entities => Set<DateOnlyTestEntity>();
    public DbSet<DateOnlyTestSubEntity> SubEntities => Set<DateOnlyTestSubEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>();
    }
}