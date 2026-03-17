namespace General.Common.Result;

public record ListDataResult<T>(IEnumerable<T> Data, int Total)
{
    public IEnumerable<string>? Errors { get; set; }
}