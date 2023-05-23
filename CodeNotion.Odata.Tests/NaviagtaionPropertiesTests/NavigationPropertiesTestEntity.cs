using System.ComponentModel.DataAnnotations.Schema;

namespace CodeNotion.Odata.Tests.NaviagtaionPropertiesTests;

[Table(nameof(NavigationPropertiesTestDbContext.Entities))]
public class NavigationPropertiesTestEntity
{
    public int Id { get; set; }

    [ForeignKey(nameof(SubEntity))]
    public int SubEntityId { get; set; }
    public virtual NavigationPropertiesTestSubEntity SubEntity { get; set; } = default!;
}
