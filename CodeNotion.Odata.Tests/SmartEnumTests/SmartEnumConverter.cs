using Ardalis.SmartEnum;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CodeNotion.Odata.Tests.SmartEnum;

public class SmartEnumConverter<TSmartEnum, TValue> : ValueConverter<TSmartEnum, TValue>
    where TSmartEnum : SmartEnum<TSmartEnum, TValue>
    where TValue : IComparable<TValue>, IEquatable<TValue>
{
    public SmartEnumConverter() : base(
        @enum => @enum.Value,
        value => SmartEnum<TSmartEnum, TValue>.FromValue(value))
    {
    }
}