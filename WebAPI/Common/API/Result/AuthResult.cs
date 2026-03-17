namespace WebAPI.Common.API.Result;

public record AuthResult(bool IsSuccess, string AccessToken, IEnumerable<string>? Messages);