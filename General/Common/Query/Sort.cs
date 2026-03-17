namespace General.Common.Query;

public class Sort
{
    public required string Field { get; set; }
    
    public required string Direction { get; set; }

    public string ToExpression()
    {
        return $"{Field} {Direction}";
    }
}