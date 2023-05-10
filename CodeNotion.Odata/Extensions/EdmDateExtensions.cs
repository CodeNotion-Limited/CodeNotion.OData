using Microsoft.OData.Edm;
using System;

namespace CodeNotion.Odata.Extensions;

internal static class EdmDateExtensions
{
    public static DateOnly ToDateOnly(this Date edmDate) => new(edmDate.Year, edmDate.Month, edmDate.Day);
}
