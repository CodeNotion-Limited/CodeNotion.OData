using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodeNotion.Odata;

[SuppressMessage("ReSharper", "ConvertTypeCheckToNullCheck")]
public class ODataService
{
    private static readonly MethodInfo ComposeOrderingMethod = typeof(ODataService).GetMethod(nameof(ComposeOrdering), BindingFlags.NonPublic | BindingFlags.Static)!;
    private readonly ODataQuerySettings _settings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ODataService(ODataQuerySettings settings, IHttpContextAccessor httpContextAccessor)
    {
        _settings = settings;
        _httpContextAccessor = httpContextAccessor;
    }

    // ReSharper disable SuspiciousTypeConversion.Global
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

        List<TEntity> items;
        if (query is IAsyncEnumerable<TEntity>)
        {
            items = await query.ToListAsync(_httpContextAccessor.HttpContext!.RequestAborted);
        }
        else
        {
            items = query.ToList();
        }

        return new ManagedPageResult<TEntity>(items, null, count);
    }

    private IQueryable<TEntity> ApplyOdata<TEntity>(IQueryable<TEntity> source, ODataQueryOptions<TEntity> queryOptions)
    {
        var query = (IQueryable)ApplyOrderBy(source, queryOptions);
        query = queryOptions.Filter?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Apply?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.SelectExpand?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Skip?.ApplyTo(query, _settings) ?? query;
        query = queryOptions.Top?.ApplyTo(query, _settings) ?? query;
        return (IQueryable<TEntity>)query;
    }

    private static IQueryable<TEntity> ApplyOrderBy<TEntity>(IQueryable<TEntity> source, ODataQueryOptions<TEntity> queryOptions)
    {
        if (queryOptions.OrderBy == null)
        {
            return source;
        }

        var entityType = typeof(TEntity);
        var tokens = queryOptions.OrderBy.RawValue.Split(" ");
        var property = ToUpperFirstChar(tokens[0]);
        var descending = tokens.LastOrDefault()?.ToLowerInvariant() == "desc";
        var propertyInfo = entityType.GetProperty(property);

        if (propertyInfo == null)
        {
            throw new InvalidOperationException($"Cannot find property {property} on type {entityType.Name}");
        }

        // uncomment this in case of Date type
        // if (propertyInfo.PropertyType != typeof(Date) && propertyInfo.PropertyType != typeof(Date?))
        // {
        //     return queryOptions.OrderBy.ApplyTo(source, _settings);
        // }

        return (IQueryable<TEntity>)ComposeOrderingMethod
            .MakeGenericMethod(entityType, propertyInfo.PropertyType)
            .Invoke(null, new object?[] { source, propertyInfo, @descending, entityType })!;
    }

    private static string ToUpperFirstChar(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        return char.ToUpper(s[0]) + s[1..];
    }

    private static IQueryable<TEntity> ComposeOrdering<TEntity, TProperty>(IQueryable<TEntity> source, PropertyInfo property, bool descending, Type? entityType = null)
    {
        var parameter = Expression.Parameter(entityType ?? typeof(TEntity), "x");
        var propertyExpression = Expression.Property(parameter, property);
        var lambda = Expression.Lambda<Func<TEntity, TProperty>>(propertyExpression, parameter);
        return descending ? source.OrderByDescending(lambda) : source.OrderBy(lambda);
    }

    private static ODataQuerySettings GetDefaultODataQuerySettings(ODataQuerySettings? settings, IServiceProvider serviceProvider)
    {
        var defaultSettings = serviceProvider.GetRequiredService<ODataQuerySettings>();

        settings ??= defaultSettings;
        settings.PageSize ??= defaultSettings.PageSize;

        return settings;
    }
}