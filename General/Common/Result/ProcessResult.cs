namespace General.Common.Result;

public record ProcessResult(bool IsSuccess, IEnumerable<string>? Messages)
{
    
}