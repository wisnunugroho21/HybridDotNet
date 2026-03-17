using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common.API;
using WebAPI.Features.Services;

namespace WebAPI.Features.Endpoints.User;

public class UpdateUser : IEndpoint
{
    public record Request(General.Data.Types.User User);

    public record OkResponse(string Message);
    
    public record ServerErrorResponse(IEnumerable<string>? Messages);

    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/", Handle)
        .WithName(nameof(UpdateUser))
        .WithSummary("Create a new User");

    private static async Task<Results<Ok<OkResponse>, InternalServerError<ServerErrorResponse>>> Handle([FromBody] Request request, [FromServices] UserService service,
        CancellationToken cancellationToken)
    {
        var result = await service.Update(request.User, cancellationToken);

        return result.IsSuccess 
            ? TypedResults.Ok(new OkResponse("Successfully updated")) 
            : TypedResults.InternalServerError(new ServerErrorResponse(result.Messages));
    }
}