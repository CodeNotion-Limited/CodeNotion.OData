using Ardalis.SmartEnum;

namespace CodeNotion.Odata.Tests.SmartEnum;

public class TestIntSmartEnum : SmartEnum<TestIntSmartEnum>
{
    public static readonly TestIntSmartEnum Member1 = new(nameof(Member1), 1);
    public static readonly TestIntSmartEnum Member2 = new(nameof(Member2), 2);
    public static readonly TestIntSmartEnum Member3 = new(nameof(Member3), 3);

    public TestIntSmartEnum(string name, int value) : base(name, value)
    {
    }
}