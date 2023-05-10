using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeNotion.Odata;

public interface IManagedPageResult
{
    public long? TotalCount { get; set; }
    public Uri? NextPageLink { get; set; }
}

public record ManagedPageResult(IEnumerable Items, Uri? NextPageLink = null, long? TotalCount = null) : IManagedPageResult
{
    public IEnumerable Items { get; set; } = Items;
    public long? TotalCount { get; set; } = TotalCount;
    public Uri? NextPageLink { get; set; } = NextPageLink;
}

public record ManagedPageResult<T>(IEnumerable<T> Items, Uri? NextPageLink = null, long? TotalCount = null) : IManagedPageResult
{
    public IEnumerable<T> Items { get; set; } = Items;
    public long? TotalCount { get; set; } = TotalCount;
    public Uri? NextPageLink { get; set; } = NextPageLink;
}