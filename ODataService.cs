using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CodeNotion.Odata.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata;

[SuppressMessage("ReSharper", "ConvertTypeCheckToNullCheck")]
public class ODataService
{
    private readonly ODataQuerySettings _settings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly FieldInfo? ParserField = typeof(ODataQueryOptions).GetField("_queryOptionParser", BindingFlags.NonPublic | BindingFlags.Instance);

    public ODataService(ODataQuerySettings settings, IHttpContextAccessor httpContextAccessor)
    {
        _settings = settings;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ManagedPageResult<TEntity>> ToPagedResultAsync<TEntity>(IQueryable<TEntity> source, ODataQueryOptions<TEntity> queryOptions)
    {
        InterceptParser(queryOptions);
        var query = ApplyOdata(source, queryOptions);

        var count = await GetCount(source, queryOptions);
        var items = await GetItems(queryOptions, query);

        return new ManagedPageResult<TEntity>(items, null, count);
    }

    private async Task<TEntity[]> GetItems<TEntity>(ODataQueryOptions<TEntity> queryOptions, IQueryable<TEntity> query)
    {
        if (queryOptions.Count != null && queryOptions.Top?.Value <= 0)
        {
            return Array.Empty<TEntity>();
        }

        if (query is IAsyncEnumerable<TEntity>)
        {
            return await query.ToArrayAsync(_httpContextAccessor.HttpContext!.RequestAborted);
        }

        return query.ToArray();
    }

    private async Task<long?> GetCount<TEntity>(IQueryable<TEntity> source, ODataQueryOptions<TEntity> queryOptions)
    {
        if (queryOptions.Count == null || !bool.TryParse(queryOptions.Count.RawValue, out var shouldCount) || !shouldCount)
        {
            return default;
        }

        var countSource = (IQueryable<TEntity>?) queryOptions.Filter?.ApplyTo(source, _settings) ?? source;
        if (countSource is IAsyncEnumerable<TEntity>)
        {
            return await countSource.LongCountAsync(_httpContextAccessor.HttpContext!.RequestAborted);
        }

        return countSource.LongCount();
    }

    private IQueryable<TEntity> ApplyOdata<TEntity>(IQueryable<TEntity> source, ODataQueryOptions<TEntity> queryOptions)
    {
        var query = (IQueryable) source;
        query = queryOptions.OrderBy?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Filter?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Apply?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.SelectExpand?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Skip?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Top?.ApplyTo(query, _settings) ?? query;
        return (IQueryable<TEntity>) query;
    }

    /// <summary>
    /// We intercept the parser to allow the injection of the IntAsEnumODataUriResolver
    /// </summary>
    private static void InterceptParser<TEntity>(ODataQueryOptions<TEntity> queryOptions)
    {
        var parser = (ODataQueryOptionParser?) ParserField?.GetValue(queryOptions);
        if (parser == null)
        {
            throw new InvalidOperationException("Could not get ODataQueryOptionParser from ODataQueryOptions");
        }

        parser.Resolver = new IntAsEnumODataUriResolver(new ODataUriResolver())
        {
            EnableCaseInsensitive = true
        };
    }
}