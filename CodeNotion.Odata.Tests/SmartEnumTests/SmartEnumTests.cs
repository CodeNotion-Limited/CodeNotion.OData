using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata.Tests.SmartEnum;

public class SmartEnumTests
{
    private readonly IServiceProvider _serviceProvider;

    public SmartEnumTests()
    {
        var services = new ServiceCollection()
            .AddApplicationOData()
            .AddHttpContextAccessor()
            .AddDbContext<SmartEnumTestDbContext>();

        _serviceProvider = services.BuildServiceProvider().CreateScope().ServiceProvider;
    }

    private ODataQueryOptions<SmartEnumTestEntity> CreateODataQueryOptions(string odataQueryString)
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<SmartEnumTestEntity>(nameof(SmartEnumTestDbContext.Entities));

        var edmModel = modelBuilder.GetEdmModel();
        var httpRequest = new DefaultHttpContext().Request;
        httpRequest.QueryString = new QueryString("?" + odataQueryString);
        httpRequest.HttpContext.RequestServices = _serviceProvider;

        var entityType = edmModel.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(e => e.Name == nameof(SmartEnumTestDbContext.Entities));
        var entitySet = edmModel.EntityContainer.FindEntitySet(nameof(SmartEnumTestDbContext.Entities));
        var odataQueryContext = new ODataQueryContext(edmModel, typeof(SmartEnumTestEntity), new ODataPath(new EntitySetSegment(entitySet)));

        return new ODataQueryOptions<SmartEnumTestEntity>(odataQueryContext, httpRequest);
    }

    [Fact]
    public void Should_GenerateCorrectSql_When_OrderBySmartEnum()
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<SmartEnumTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$orderby={nameof(SmartEnumTestEntity.NullableSmartEnumProperty)}");

        // variables to keep SQL strict & readable
        var Entities = nameof(SmartEnumTestDbContext.Entities);
        var e = nameof(SmartEnumTestDbContext.Entities).ToLower().First();
        var Id = nameof(SmartEnumTestEntity.Id);
        var NullableSmartEnumProperty = nameof(SmartEnumTestEntity.NullableSmartEnumProperty);

        var expectedSql = @$"SELECT [{e}].[{Id}], [{e}].[{NullableSmartEnumProperty}]
FROM [{Entities}] AS [{e}]
ORDER BY [{e}].[{NullableSmartEnumProperty}]";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.NullableSmartEnumProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public void Should_GenerateCorrectSql_When_OrderBySmartEnumDescending()
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<SmartEnumTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$orderby={nameof(SmartEnumTestEntity.NullableSmartEnumProperty)} desc");

        // variables to keep SQL strict & readable
        var Entities = nameof(SmartEnumTestDbContext.Entities);
        var e = nameof(SmartEnumTestDbContext.Entities).ToLower().First();
        var Id = nameof(SmartEnumTestEntity.Id);
        var NullableSmartEnumProperty = nameof(SmartEnumTestEntity.NullableSmartEnumProperty);

        var expectedSql = @$"SELECT [{e}].[{Id}], [{e}].[{NullableSmartEnumProperty}]
FROM [{Entities}] AS [{e}]
ORDER BY [{e}].[{NullableSmartEnumProperty}] DESC";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.NullableSmartEnumProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }
}