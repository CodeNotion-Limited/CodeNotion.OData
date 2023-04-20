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
        modelBuilder.EntitySet<SmartEnumTestEntity>(nameof(SmartEnumTestDbContext.TestEntities));

        var edmModel = modelBuilder.GetEdmModel();
        var httpRequest = new DefaultHttpContext().Request;
        httpRequest.QueryString = new QueryString("?" + odataQueryString);
        httpRequest.HttpContext.RequestServices = _serviceProvider;

        var entityType = edmModel.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(e => e.Name == nameof(SmartEnumTestDbContext.TestEntities));
        var entitySet = edmModel.EntityContainer.FindEntitySet(nameof(SmartEnumTestDbContext.TestEntities));
        var odataQueryContext = new ODataQueryContext(edmModel, typeof(SmartEnumTestEntity), new ODataPath(new EntitySetSegment(entitySet)));

        return new ODataQueryOptions<SmartEnumTestEntity>(odataQueryContext, httpRequest);
    }

    [Fact]
    public void Should_GenerateCorrectSql_When_OrderBySmartEnum()
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<SmartEnumTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$orderby={nameof(SmartEnumTestEntity.SmartEnum)}");

        // variables to keep SQL strict & readable
        var TestEntities = nameof(SmartEnumTestDbContext.TestEntities);
        var t = nameof(SmartEnumTestDbContext.TestEntities).ToLower().First();
        var Id = nameof(SmartEnumTestEntity.Id);
        var SmartEnum = nameof(SmartEnumTestEntity.SmartEnum);

        var expectedSql = @$"SELECT [{t}].[{Id}], [{t}].[{SmartEnum}]
FROM [{TestEntities}] AS [{t}]
ORDER BY [{t}].[{SmartEnum}]";

        // Act
        var actualSql = odataService.ApplyOdata(context.TestEntities, odataOptions).ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql);
    }

    [Fact]
    public void Should_GenerateCorrectSql_When_OrderBySmartEnumDescending()
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<SmartEnumTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$orderby={nameof(SmartEnumTestEntity.SmartEnum)} desc");

        // variables to keep SQL strict & readable
        var TestEntities = nameof(SmartEnumTestDbContext.TestEntities);
        var t = nameof(SmartEnumTestDbContext.TestEntities).ToLower().First();
        var Id = nameof(SmartEnumTestEntity.Id);
        var SmartEnum = nameof(SmartEnumTestEntity.SmartEnum);

        var expectedSql = @$"SELECT [{t}].[{Id}], [{t}].[{SmartEnum}]
FROM [{TestEntities}] AS [{t}]
ORDER BY [{t}].[{SmartEnum}] DESC";

        // Act
        var actualSql = odataService.ApplyOdata(context.TestEntities, odataOptions).ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql);
    }
}