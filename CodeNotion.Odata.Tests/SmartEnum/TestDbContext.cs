using Microsoft.EntityFrameworkCore;

namespace CodeNotion.Odata.Tests.SmartEnum;

public class TestDbContext : DbContext
{
    public DbSet<TestEntity> TestEntities => Set<TestEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<TestIntSmartEnum>()
            .HaveConversion<SmartEnumConverter<TestIntSmartEnum, int>>();
    }
}