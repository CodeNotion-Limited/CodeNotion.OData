using CodeNotion.Odata.Resolvers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata;

public static class ODataConfiguration
{
    public static IServiceCollection AddApplicationOData(this IServiceCollection services) =>
        services
            .AddScoped<ODataService>()
            .AddSingleton<ODataUriResolver>(sp => new IntAsEnumODataUriResolver(sp.GetRequiredService<ODataUriResolver>()))
            .AddSingleton(_ => ODataQuerySettings);

    private static ODataQuerySettings ODataQuerySettings => new()
    {
        PageSize = null,
        EnableConstantParameterization = true
    };
}