using Microsoft.EntityFrameworkCore;

namespace CodeNotion.Odata.Tests.SmartEnum;

public class SmartEnumTestDbContext : DbContext
{
    public DbSet<SmartEnumTestEntity> TestEntities => Set<SmartEnumTestEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<TestSmartEnum>()
            .HaveConversion<SmartEnumConverter<TestSmartEnum, int>>();
    }
}