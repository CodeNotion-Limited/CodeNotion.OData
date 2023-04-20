using System.ComponentModel.DataAnnotations.Schema;

namespace CodeNotion.Odata.Tests.SmartEnum;

[Table(nameof(TestDbContext.TestEntities))]
public class TestEntity
{
    public int Id { get; set; }
    public TestIntSmartEnum? SmartEnum { get; set; }
}