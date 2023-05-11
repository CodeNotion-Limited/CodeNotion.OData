using System.ComponentModel.DataAnnotations.Schema;

namespace CodeNotion.Odata.Tests.DateTimeTests;

[Table(nameof(DateTimeTestDbContext.Entities))]
public class DateTimeTestEntity
{
    public int Id { get; set; }
    public DateTime? NullableDateTimeProperty { get; set; }
    public DateTime DateTimeProperty { get; set; }

    public int SubEntityId { get; set; }
    public virtual DateTimeTestSubEntity SubEntity { get; set; } = default!;
}
