using CodeNotion.Odata.Tests.DateTimeTests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata.Tests.DateTimeTests;

public class DateTimeTests
{
    private readonly IServiceProvider _serviceProvider;

    public DateTimeTests()
    {
        var services = new ServiceCollection()
            .AddApplicationOData()
            .AddHttpContextAccessor()
            .AddDbContext<DateTimeTestDbContext>();

        _serviceProvider = services.BuildServiceProvider().CreateScope().ServiceProvider;
    }

    private ODataQueryOptions<DateTimeTestEntity> CreateODataQueryOptions(string odataQueryString)
    {
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<DateTimeTestEntity>(nameof(DateTimeTestDbContext.Entities));
        modelBuilder.EntitySet<DateTimeTestSubEntity>(nameof(DateTimeTestDbContext.SubEntities));

        var edmModel = modelBuilder.GetEdmModel();
        var httpRequest = new DefaultHttpContext().Request;
        httpRequest.QueryString = new QueryString("?" + odataQueryString);
        httpRequest.HttpContext.RequestServices = _serviceProvider;

        var entityType = edmModel.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(e => e.Name == nameof(DateTimeTestDbContext.Entities));
        var entitySet = edmModel.EntityContainer.FindEntitySet(nameof(DateTimeTestDbContext.Entities));
        var odataQueryContext = new ODataQueryContext(edmModel, typeof(DateTimeTestEntity), new ODataPath(new EntitySetSegment(entitySet)));

        return new ODataQueryOptions<DateTimeTestEntity>(odataQueryContext, httpRequest);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Should_GenerateCorrectSql_When_FilterByDateTimePropertyEqualsSpecificDate(bool isDateEnclosedInQuotes)
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<DateTimeTestDbContext>();
        var dateString = isDateEnclosedInQuotes ? "'2023-02-01T13:37:15Z'" : "2023-02-01T13:37:15Z";
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(DateTimeTestEntity.DateTimeProperty)} eq {dateString}");

        // variables to keep SQL strict & readable
        var Entities = nameof(DateTimeTestDbContext.Entities);
        var e = nameof(DateTimeTestDbContext.Entities).ToLower().First();
        var Id = nameof(DateTimeTestEntity.Id);
        var DateTimeProperty = nameof(DateTimeTestEntity.DateTimeProperty);

        var expectedSql = @$"DECLARE @__TypedProperty_0 datetime2 = '2023-02-01T14:37:15.0000000';

SELECT [{e}].[{Id}], [{e}].[{DateTimeProperty}]
FROM [{Entities}] AS [{e}]
WHERE [{e}].[{DateTimeProperty}] = @__TypedProperty_0";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.DateTimeProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public void Should_GenerateCorrectSql_When_FilterByNullableDateTimePropertyEqualsNull()
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<DateTimeTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(DateTimeTestEntity.NullableDateTimeProperty)} eq null");

        // variables to keep SQL strict & readable
        var Entities = nameof(DateTimeTestDbContext.Entities);
        var e = nameof(DateTimeTestDbContext.Entities).ToLower().First();
        var Id = nameof(DateTimeTestEntity.Id);
        var NullableDateTimeProperty = nameof(DateTimeTestEntity.NullableDateTimeProperty);

        var expectedSql = @$"SELECT [{e}].[{Id}], [{e}].[{NullableDateTimeProperty}]
FROM [{Entities}] AS [{e}]
WHERE [{e}].[{NullableDateTimeProperty}] IS NULL";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.NullableDateTimeProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Should_GenerateCorrectSql_When_FilterByNullableDateTimePropertyEqualsSpecificDate(bool isDateEnclosedInQuotes)
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<DateTimeTestDbContext>();
        var dateString = isDateEnclosedInQuotes ? "'2023-02-01T13:37:15Z'" : "2023-02-01T13:37:15Z";
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(DateTimeTestEntity.NullableDateTimeProperty)} eq {dateString}");

        // variables to keep SQL strict & readable
        var Entities = nameof(DateTimeTestDbContext.Entities);
        var e = nameof(DateTimeTestDbContext.Entities).ToLower().First();
        var Id = nameof(DateTimeTestEntity.Id);
        var NullableDateTimeProperty = nameof(DateTimeTestEntity.NullableDateTimeProperty);

        var expectedSql = @$"DECLARE @__TypedProperty_0 datetime2 = '2023-02-01T14:37:15.0000000';

SELECT [{e}].[{Id}], [{e}].[{NullableDateTimeProperty}]
FROM [{Entities}] AS [{e}]
WHERE [{e}].[{NullableDateTimeProperty}] = @__TypedProperty_0";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.NullableDateTimeProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Should_GenerateCorrectSql_When_FilterBySubEntityDateTimePropertyEqualsSpecificDate(bool isDateEnclosedInQuotes)
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<DateTimeTestDbContext>();
        var dateString = isDateEnclosedInQuotes ? "'2023-02-01T13:37:15Z'" : "2023-02-01T13:37:15Z";
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(DateTimeTestEntity.SubEntity)}/{nameof(DateTimeTestEntity.SubEntity.DateTimeProperty)} eq {dateString}");

        // variables to keep SQL strict & readable
        var Entities = nameof(DateTimeTestDbContext.Entities);
        var e = nameof(DateTimeTestDbContext.Entities).ToLower().First();
        var eId = nameof(DateTimeTestEntity.Id);
        var SubEntityId = nameof(DateTimeTestEntity.SubEntityId);
        var SubEntities = nameof(DateTimeTestDbContext.SubEntities);
        var s = nameof(DateTimeTestDbContext.SubEntities).ToLower().First();
        var sId = nameof(DateTimeTestSubEntity.Id);
        var DateTimeProperty = nameof(DateTimeTestEntity.DateTimeProperty);

        var expectedSql = @$"DECLARE @__TypedProperty_0 datetime2 = '2023-02-01T14:37:15.0000000';

SELECT [{e}].[{eId}], [{s}].[{DateTimeProperty}]
FROM [{Entities}] AS [{e}]
INNER JOIN [{SubEntities}] AS [{s}] ON [{e}].[{SubEntityId}] = [{s}].[{sId}]
WHERE [{s}].[{DateTimeProperty}] = @__TypedProperty_0";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.SubEntity.DateTimeProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }

    [Fact]
    public void Should_GenerateCorrectSql_When_FilterBySubEntityNullableDateTimePropertyEqualsNull()
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<DateTimeTestDbContext>();
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(DateTimeTestEntity.SubEntity)}/{nameof(DateTimeTestEntity.SubEntity.NullableDateTimeProperty)} eq null");

        // variables to keep SQL strict & readable
        var Entities = nameof(DateTimeTestDbContext.Entities);
        var e = nameof(DateTimeTestDbContext.Entities).ToLower().First();
        var eId = nameof(DateTimeTestEntity.Id);
        var SubEntityId = nameof(DateTimeTestEntity.SubEntityId);
        var SubEntities = nameof(DateTimeTestDbContext.SubEntities);
        var s = nameof(DateTimeTestDbContext.SubEntities).ToLower().First();
        var sId = nameof(DateTimeTestSubEntity.Id);
        var NullableDateTimeProperty = nameof(DateTimeTestEntity.NullableDateTimeProperty);

        var expectedSql = @$"SELECT [{e}].[{eId}], [{s}].[{NullableDateTimeProperty}]
FROM [{Entities}] AS [{e}]
INNER JOIN [{SubEntities}] AS [{s}] ON [{e}].[{SubEntityId}] = [{s}].[{sId}]
WHERE [{s}].[{NullableDateTimeProperty}] IS NULL";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.SubEntity.NullableDateTimeProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Should_GenerateCorrectSql_When_FilterBySubEntityNullableDateTimePropertyEqualsSpecificDate(bool isDateEnclosedInQuotes)
    {
        // Arrange
        var odataService = _serviceProvider.GetRequiredService<ODataService>();
        var context = _serviceProvider.GetRequiredService<DateTimeTestDbContext>();
        var dateString = isDateEnclosedInQuotes ? "'2023-02-01T13:37:15Z'" : "2023-02-01T13:37:15Z";
        var odataOptions = CreateODataQueryOptions($"$filter={nameof(DateTimeTestEntity.SubEntity)}/{nameof(DateTimeTestEntity.SubEntity.NullableDateTimeProperty)} eq {dateString}");

        // variables to keep SQL strict & readable
        var Entities = nameof(DateTimeTestDbContext.Entities);
        var e = nameof(DateTimeTestDbContext.Entities).ToLower().First();
        var eId = nameof(DateTimeTestEntity.Id);
        var SubEntityId = nameof(DateTimeTestEntity.SubEntityId);
        var SubEntities = nameof(DateTimeTestDbContext.SubEntities);
        var s = nameof(DateTimeTestDbContext.SubEntities).ToLower().First();
        var sId = nameof(DateTimeTestSubEntity.Id);
        var NullableDateTimeProperty = nameof(DateTimeTestEntity.NullableDateTimeProperty);

        var expectedSql = @$"DECLARE @__TypedProperty_0 datetime2 = '2023-02-01T14:37:15.0000000';

SELECT [{e}].[{eId}], [{s}].[{NullableDateTimeProperty}]
FROM [{Entities}] AS [{e}]
INNER JOIN [{SubEntities}] AS [{s}] ON [{e}].[{SubEntityId}] = [{s}].[{sId}]
WHERE [{s}].[{NullableDateTimeProperty}] = @__TypedProperty_0";

        // Act
        var actualSql = odataService.ApplyOdata(context.Entities, odataOptions)
            .Select(x => new { x.Id, x.SubEntity.NullableDateTimeProperty }) // explicitly specifying properties
            .ToQueryString();

        // Assert
        Assert.Equal(expectedSql, actualSql, ignoreLineEndingDifferences: true);
    }
}