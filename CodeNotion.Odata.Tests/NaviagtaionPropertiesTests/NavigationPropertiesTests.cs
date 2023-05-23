using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata.Tests.NaviagtaionPropertiesTests;

public class NavigationPropertiesTests
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationPropertiesTests()
    {
        var services = new ServiceCollection()
            .AddApplicationOData()
            .AddHttpContextAccessor()
            .AddDbContext<NavigationPropertiesTestDbContext>();

        _serviceProvider = services.BuildServiceProvider().CreateScope().ServiceProvider;
    }

    private ODataQueryOptions<NavigationPropertiesTestEntity> CreateODataQueryOptions(string odataQueryString)
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<NavigationPropertiesTestEntity>(nameof(NavigationPropertiesTestDbContext.Entities));
        modelBuilder.EntitySet<NavigationPropertiesTestSubEntity>(nameof(NavigationPropertiesTestDbContext.SubEntities));

        var edmModel = modelBuilder.GetEdmModel();
        var httpRequest = new DefaultHttpContext().Request;
        httpRequest.QueryString = new QueryString("?" + odataQueryString);
        httpRequest.HttpContext.RequestServices = _serviceProvider;

        var entityType = edmModel.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(e => e.Name == nameof(NavigationPropertiesTestDbContext.Entities));
        var entitySet = edmModel.EntityContainer.FindEntitySet(nameof(NavigationPropertiesTestDbContext.Entities));
        var odataQueryContext = new ODataQueryContext(edmModel, typeof(NavigationPropertiesTestEntity), new ODataPath(new EntitySetSegment(entitySet)));

        return new ODataQueryOptions<NavigationPropertiesTestEntity>(odataQueryContext, httpRequest);
    }

    [Theory]
    [InlineData(ComparingOperator.Equal)]
    [InlineData(ComparingOperator.NotEqual)]
    [InlineData(ComparingOperator.LessThan)]
    [InlineData(ComparingOperator.GreaterThan)]
    [InlineData(ComparingOperator.LessThanOrEqual)]
    [InlineData(ComparingOperator.GreaterThanOrEqual)]
    public void Should_GenerateCorrectSql_When_FilterBySubEntityIdProperty(ComparingOperator comparingOperator)
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<NavigationPropertiesTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(NavigationPropertiesTestEntity.SubEntity)}/{nameof(NavigationPropertiesTestEntity.SubEntity.Id)} {comparingOperator.ToODataString()} 5");

        // variables to keep SQL strict & readable
        var Entities = nameof(NavigationPropertiesTestDbContext.Entities);
        var e = nameof(NavigationPropertiesTestDbContext.Entities).ToLower().First();
        var eId = nameof(NavigationPropertiesTestEntity.Id);
        var SubEntityId = nameof(NavigationPropertiesTestEntity.SubEntityId);
        var SubEntities = nameof(NavigationPropertiesTestDbContext.SubEntities);
        var s = nameof(NavigationPropertiesTestDbContext.SubEntities).ToLower().First();
        var sId = nameof(NavigationPropertiesTestSubEntity.Id);

        var expectedSql = @$"DECLARE @__TypedProperty_0 int = 5;

SELECT [{e}].[{eId}]
FROM [{Entities}] AS [{e}]
INNER JOIN [{SubEntities}] AS [{s}] ON [{e}].[{SubEntityId}] = [{s}].[{sId}]
WHERE [{s}].[{sId}] {comparingOperator.ToSqlString()} @__TypedProperty_0";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }
}