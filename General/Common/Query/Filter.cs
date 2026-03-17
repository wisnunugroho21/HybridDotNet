using System.Reflection;

namespace General.Common.Query;

public class FilterConstraint
{
    public string? Operator { get; set; }
    
    public object? Keyword { get; set; }
}

public class Filter
{
    public string? Field { get; set; }
    
    public string? Logic { get; set; }

    public List<FilterConstraint> Constraints { get; set; } = [];

    private static readonly Dictionary<string, string> Operators = new()
    {
        { "eq", "=" },
        { "neq", "!=" },
        { "lt", "<" },
        { "lte", "<=" },
        { "gt", ">" },
        { "gte", ">=" },
        { "starts-with", "StartsWith" },
        { "ends-with", "EndsWith" },
        { "contains", "Contains" },
        { "does-not-contain", "Contains" },
        { "is-empty", "" },
        { "is-not-empty", "!" },
        { "is-null", "=" },
        { "is-not-null", "!=" },
        { "is-null-or-empty", "" },
        { "is-not-null-or-empty", "!" }
    };

    public string ToExpression(int? filterIndex)
    {
        List<string> expressions = [];

        for (var i = 0; i < Constraints.Count; i++)
        {
            var constraint = Constraints[i];
            var comparison = Operators[constraint.Operator ?? ""];
            
            var expression = constraint.Operator switch
            {
                "does-not-contain" when constraint.Keyword!.GetType().IsArray && filterIndex is not null =>
                    $"!@{filterIndex + i}.{comparison}({Field})",
                "contains" when constraint.Keyword!.GetType().IsArray && filterIndex is not null =>
                    $"@{filterIndex + i}.{comparison}({Field})",
                "does-not-contain" => $"!{Field}.{comparison}(@{filterIndex + i})",
                "contains" or "starts-with" or "ends-with" => $"{Field}.{comparison}(@{filterIndex + i})",
                "is-empty" or "is-not-empty" => $"{Field} {comparison} String.Empty",
                "is-null" or "is-not-null" => $"{Field} {comparison} null",
                "is-null-or-empty" or "is-not-null-or-empty" => $"{comparison}String.IsNullOrEmpty({Field})",
                _ => $"{Field} {comparison} @{filterIndex + i}"
            };
            
            expressions.Add(expression);
        }

        if (expressions.Count == 0)
            return "";

        var predicate =
            string.Join(string.Equals(Logic, "or", StringComparison.InvariantCultureIgnoreCase) ? " OR " : " AND ",
                expressions);
        
        return $"({predicate})";
    }

    public static Type GetLastPropertyType(Type type, string field)
    {
        var currentType = type;

        foreach (var propertyName in field.Split('.'))
        {
            var property = currentType.GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
                currentType = property.PropertyType;
        }

        return currentType;
    }
}