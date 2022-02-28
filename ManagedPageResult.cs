using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeNotion.Odata;

public interface IManagedPageResult
{
    public long? TotalCount { get; set; }
    public Uri? NextPageLink { get; set; }
}

public class ManagedPageResult : IManagedPageResult
{
    public IEnumerable Items { get; set; }
    public long? TotalCount { get; set; }
    public Uri? NextPageLink { get; set; }

    public ManagedPageResult(IEnumerable items, Uri? nextPageLink = null, long? count = null)
    {
        Items = items;
        TotalCount = count;
        NextPageLink = nextPageLink;
    }
}

public class ManagedPageResult<T> : IManagedPageResult
{
    public IEnumerable<T> Items { get; set; }
    public long? TotalCount { get; set; }
    public Uri? NextPageLink { get; set; }

    public ManagedPageResult(IEnumerable<T> items, Uri? nextPageLink = null, long? count = null)
    {
        Items = items;
        TotalCount = count;
        NextPageLink = nextPageLink;
    }
}