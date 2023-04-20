using Ardalis.SmartEnum;

namespace CodeNotion.Odata.Tests.SmartEnum;

public class TestSmartEnum : SmartEnum<TestSmartEnum>
{
    public static readonly TestSmartEnum Member1 = new(nameof(Member1), 1);
    public static readonly TestSmartEnum Member2 = new(nameof(Member2), 2);
    public static readonly TestSmartEnum Member3 = new(nameof(Member3), 3);

    public TestSmartEnum(string name, int value) : base(name, value)
    {
    }
}