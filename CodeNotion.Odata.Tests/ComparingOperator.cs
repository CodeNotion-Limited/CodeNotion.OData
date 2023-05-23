namespace CodeNotion.Odata.Tests;

public enum ComparingOperator
{
    Equal,
    NotEqual,
    LessThan,
    GreaterThan,
    LessThanOrEqual,
    GreaterThanOrEqual,
}

internal static class ComparingOperatorExtensions
{
    public static string ToODataString(this ComparingOperator @operator)
        => @operator switch
        {
            ComparingOperator.Equal              => "eq",
            ComparingOperator.NotEqual           => "ne",
            ComparingOperator.LessThan           => "lt",
            ComparingOperator.GreaterThan        => "gt",
            ComparingOperator.LessThanOrEqual    => "le",
            ComparingOperator.GreaterThanOrEqual => "ge",

            _ => throw new NotSupportedException()
        };

    public static string ToSqlString(this ComparingOperator @operator)
        => @operator switch
        {
            ComparingOperator.Equal              => "=",
            ComparingOperator.NotEqual           => "<>",
            ComparingOperator.LessThan           => "<",
            ComparingOperator.GreaterThan        => ">",
            ComparingOperator.LessThanOrEqual    => "<=",
            ComparingOperator.GreaterThanOrEqual => ">=",

            _ => throw new NotSupportedException()
        };
}
