using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace CodeNotion.Odata;

[SuppressMessage("ReSharper", "ConvertTypeCheckToNullCheck")]
public class ODataService
{
    private readonly ODataQuerySettings _settings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ODataService(ODataQuerySettings settings, IHttpContextAccessor httpContextAccessor)
    {
        _settings = settings;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ManagedPageResult<TEntity>> ToPagedResultAsync<TEntity>(IQueryable<TEntity> source, ODataQueryOptions<TEntity> queryOptions)
    {
        var query = ApplyOdata(source, queryOptions);

        var count = default(long?);
        if (queryOptions.Count != null && bool.TryParse(queryOptions.Count.RawValue, out var shouldCount) && shouldCount)
        {
            var countSource = (IQueryable<TEntity>?)queryOptions.Filter?.ApplyTo(source, _settings) ?? source;

            if (countSource is IAsyncEnumerable<TEntity>)
            {
                count = await countSource.LongCountAsync(_httpContextAccessor.HttpContext!.RequestAborted);
            }
            else
            {
                count = countSource.LongCount();
            }
        }

        var items = new List<TEntity>();
        if (queryOptions.Count == null || queryOptions.Top.Value > 0)
        {
            if (query is IAsyncEnumerable<TEntity>)
            {
                items = await query.ToListAsync(_httpContextAccessor.HttpContext!.RequestAborted);
            }
            else
            {
                items = query.ToList();
            }
        }

        return new ManagedPageResult<TEntity>(items, null, count);
    }

    private IQueryable<TEntity> ApplyOdata<TEntity>(IQueryable<TEntity> source, ODataQueryOptions<TEntity> queryOptions)
    {
        var query = (IQueryable)source;
        query = queryOptions.OrderBy?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Filter?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Apply?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.SelectExpand?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Skip?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Top?.ApplyTo(query, _settings) ?? query;
        return (IQueryable<TEntity>)query;
    }
}