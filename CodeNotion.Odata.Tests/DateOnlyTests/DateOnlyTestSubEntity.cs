using System.ComponentModel.DataAnnotations.Schema;

namespace CodeNotion.Odata.Tests.DateOnlyTests;

[Table(nameof(DateOnlyTestDbContext.SubEntities))]
public class DateOnlyTestSubEntity
{
    public int Id { get; set; }
    public DateOnly? NullableDateOnlyProperty { get; set; }
    public DateOnly DateOnlyProperty { get; set; }
}