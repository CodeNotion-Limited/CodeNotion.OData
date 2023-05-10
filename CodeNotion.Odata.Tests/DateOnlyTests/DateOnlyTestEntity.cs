using System.ComponentModel.DataAnnotations.Schema;

namespace CodeNotion.Odata.Tests.DateOnlyTests;

[Table(nameof(DateOnlyTestDbContext.Entities))]
public class DateOnlyTestEntity
{
    public int Id { get; set; }
    public DateOnly? NullableDateOnlyProperty { get; set; }
    public DateOnly DateOnlyProperty { get; set; }

    public int SubEntityId { get; set; }
    public virtual DateOnlyTestSubEntity SubEntity { get; set; } = default!;
}
