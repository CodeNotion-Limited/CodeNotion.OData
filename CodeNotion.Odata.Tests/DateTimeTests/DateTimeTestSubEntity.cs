using System.ComponentModel.DataAnnotations.Schema;

namespace CodeNotion.Odata.Tests.DateTimeTests;

[Table(nameof(DateTimeTestDbContext.SubEntities))]
public class DateTimeTestSubEntity
{
    public int Id { get; set; }
    public DateTime? NullableDateTimeProperty { get; set; }
    public DateTime DateTimeProperty { get; set; }
}