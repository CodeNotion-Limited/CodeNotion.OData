using CodeNotion.Odata.Tests.DateOnlyTests;
using Microsoft.EntityFrameworkCore;

namespace CodeNotion.Odata.Tests.DateTimeTests;

public class DateTimeTestDbContext : DbContext
{
    public DbSet<DateTimeTestEntity> Entities => Set<DateTimeTestEntity>();
    public DbSet<DateTimeTestSubEntity> SubEntities => Set<DateTimeTestSubEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer();
    }
}