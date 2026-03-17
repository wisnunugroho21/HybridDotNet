using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common.API;
using WebAPI.Common.Auth.Services;

namespace VeryMinimalAPI.Common.Auth.Endpoints;

public class Login : IEndpoint
{
    public record Request(string Username, string Password);

    public record OkResponse(string Token, string Message);

    public record BadRequestResponse(IEnumerable<string>? Messages);

    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/login", Handle)
        .WithName(nameof(Login))
        .WithSummary("Logs in an user");

    private static async Task<Results<Ok<OkResponse>, BadRequest<BadRequestResponse>>> Handle([FromBody] Request request, [FromServices] AuthService service,
        CancellationToken cancellationToken)
    {
        var result = await service.Login(request.Username, request.Password, cancellationToken);

        if (string.IsNullOrWhiteSpace(result.AccessToken))
            return TypedResults.BadRequest(new BadRequestResponse(result.Messages));
        
        return TypedResults.Ok(new OkResponse(result.AccessToken, "Successfully logged in"));
    }
}