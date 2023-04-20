using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata.Tests.DateOnlyTests;

public class DateOnlyTests
{
    private readonly IServiceProvider _serviceProvider;

    public DateOnlyTests()
    {
        var services = new ServiceCollection()
            .AddApplicationOData()
            .AddHttpContextAccessor()
            .AddDbContext<DateOnlyTestDbContext>();

        _serviceProvider = services.BuildServiceProvider().CreateScope().ServiceProvider;
    }

    private ODataQueryOptions<DateOnlyTestEntity> CreateODataQueryOptions(string odataQueryString)
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<DateOnlyTestEntity>(nameof(DateOnlyTestDbContext.Entities));
        modelBuilder.EntitySet<DateOnlyTestSubEntity>(nameof(DateOnlyTestDbContext.SubEntities));

        var edmModel = modelBuilder.GetEdmModel();
        var httpRequest = new DefaultHttpContext().Request;
        httpRequest.QueryString = new QueryString("?" + odataQueryString);
        httpRequest.HttpContext.RequestServices = _serviceProvider;

        var entityType = edmModel.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(e => e.Name == nameof(DateOnlyTestDbContext.Entities));
        var entitySet = edmModel.EntityContainer.FindEntitySet(nameof(DateOnlyTestDbContext.Entities));
        var odataQueryContext = new ODataQueryContext(edmModel, typeof(DateOnlyTestEntity), new ODataPath(new EntitySetSegment(entitySet)));

        return new ODataQueryOptions<DateOnlyTestEntity>(odataQueryContext, httpRequest);
    }

    [Fact]
    public void Should_GenerateCorrectSql_When_FilterByDateOnlyPropertyEqualsSpecificDate()
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<DateOnlyTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(DateOnlyTestEntity.DateOnlyProperty)} eq 2023-02-01");

        // variables to keep SQL strict & readable
        var Entities = nameof(DateOnlyTestDbContext.Entities);
        var e = nameof(DateOnlyTestDbContext.Entities).ToLower().First();
        var Id = nameof(DateOnlyTestEntity.Id);
        var DateOnlyProperty = nameof(DateOnlyTestEntity.DateOnlyProperty);

        var expectedSql = @$"SELECT [{e}].[{Id}], [{e}].[{DateOnlyProperty}]
FROM [{Entities}] AS [{e}]
WHERE [{e}].[{DateOnlyProperty}] = '2023-02-01T00:00:00.0000000'";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.DateOnlyProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public void Should_GenerateCorrectSql_When_FilterByNullableDateOnlyPropertyEqualsNull()
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<DateOnlyTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(DateOnlyTestEntity.NullableDateOnlyProperty)} eq null");

        // variables to keep SQL strict & readable
        var Entities = nameof(DateOnlyTestDbContext.Entities);
        var e = nameof(DateOnlyTestDbContext.Entities).ToLower().First();
        var Id = nameof(DateOnlyTestEntity.Id);
        var NullableDateOnlyProperty = nameof(DateOnlyTestEntity.NullableDateOnlyProperty);

        var expectedSql = @$"SELECT [{e}].[{Id}], [{e}].[{NullableDateOnlyProperty}]
FROM [{Entities}] AS [{e}]
WHERE [{e}].[{NullableDateOnlyProperty}] IS NULL";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.NullableDateOnlyProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public void Should_GenerateCorrectSql_When_FilterByNullableDateOnlyPropertyEqualsSpecificDate()
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<DateOnlyTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(DateOnlyTestEntity.NullableDateOnlyProperty)} eq 2023-02-01");

        // variables to keep SQL strict & readable
        var Entities = nameof(DateOnlyTestDbContext.Entities);
        var e = nameof(DateOnlyTestDbContext.Entities).ToLower().First();
        var Id = nameof(DateOnlyTestEntity.Id);
        var NullableDateOnlyProperty = nameof(DateOnlyTestEntity.NullableDateOnlyProperty);

        var expectedSql = @$"SELECT [{e}].[{Id}], [{e}].[{NullableDateOnlyProperty}]
FROM [{Entities}] AS [{e}]
WHERE [{e}].[{NullableDateOnlyProperty}] = '2023-02-01T00:00:00.0000000'";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.NullableDateOnlyProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public void Should_GenerateCorrectSql_When_FilterBySubEntityDateOnlyPropertyEqualsSpecificDate()
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<DateOnlyTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(DateOnlyTestEntity.SubEntity)}/{nameof(DateOnlyTestEntity.SubEntity.DateOnlyProperty)} eq 2023-02-01");

        // variables to keep SQL strict & readable
        var Entities = nameof(DateOnlyTestDbContext.Entities);
        var e = nameof(DateOnlyTestDbContext.Entities).ToLower().First();
        var eId = nameof(DateOnlyTestEntity.Id);
        var SubEntityId = nameof(DateOnlyTestEntity.SubEntityId);
        var SubEntities = nameof(DateOnlyTestDbContext.SubEntities);
        var s = nameof(DateOnlyTestDbContext.SubEntities).ToLower().First();
        var sId = nameof(DateOnlyTestSubEntity.Id);
        var DateOnlyProperty = nameof(DateOnlyTestEntity.DateOnlyProperty);

        var expectedSql = @$"SELECT [{e}].[{eId}], [{s}].[{DateOnlyProperty}]
FROM [{Entities}] AS [{e}]
INNER JOIN [{SubEntities}] AS [{s}] ON [{e}].[{SubEntityId}] = [{s}].[{sId}]
WHERE [{s}].[{DateOnlyProperty}] = '2023-02-01T00:00:00.0000000'";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.SubEntity.DateOnlyProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }
}