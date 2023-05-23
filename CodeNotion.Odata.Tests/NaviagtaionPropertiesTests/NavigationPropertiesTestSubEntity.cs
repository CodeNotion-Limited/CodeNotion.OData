using System.ComponentModel.DataAnnotations.Schema;

namespace CodeNotion.Odata.Tests.NaviagtaionPropertiesTests;

[Table(nameof(NavigationPropertiesTestDbContext.SubEntities))]
public class NavigationPropertiesTestSubEntity
{
    public int Id { get; set; }
}