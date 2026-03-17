using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common.API;
using WebAPI.Features.Services;

namespace WebAPI.Features.Endpoints.User;

public class GetUser : IEndpoint
{
    public record Request(long Id);

    public record OkResponse(General.Data.Types.User? Data);
    
    public record ServerErrorResponse(IEnumerable<string>? Messages);

    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{Id:long}", Handle)
        .WithName(nameof(GetUser))
        .WithSummary("Create a new User");

    private static async Task<Results<Ok<OkResponse>, InternalServerError<ServerErrorResponse>>> Handle([AsParameters] Request request, [FromServices] UserService service,
        CancellationToken cancellationToken)
    {
        var result = await service.Get(request.Id, cancellationToken);

        return result.Errors is not null && result.Errors.Any()
            ? TypedResults.InternalServerError(new ServerErrorResponse(result.Errors))
            : TypedResults.Ok(new OkResponse(result.Data));
    }
}