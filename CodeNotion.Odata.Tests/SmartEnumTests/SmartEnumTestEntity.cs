using System.ComponentModel.DataAnnotations.Schema;

namespace CodeNotion.Odata.Tests.SmartEnum;

[Table(nameof(SmartEnumTestDbContext.Entities))]
public class SmartEnumTestEntity
{
    public int Id { get; set; }
    public TestSmartEnum? NullableSmartEnumProperty { get; set; }
}