using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common.API;
using WebAPI.Features.Services;

namespace WebAPI.Features.Endpoints.User;

public class DeleteUser : IEndpoint
{
    public record Request(long Id);

    public record OkResponse(string Message);
    
    public record ServerErrorResponse(IEnumerable<string>? Messages);

    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{Id:long}", Handle)
        .WithName(nameof(DeleteUser))
        .WithSummary("Delete a User");

    private static async Task<Results<Ok<OkResponse>, InternalServerError<ServerErrorResponse>>> Handle([AsParameters] Request request, [FromServices] UserService service,
        CancellationToken cancellationToken)
    {
        var result = await service.Delete(request.Id, cancellationToken);
        
        return result.IsSuccess 
            ? TypedResults.Ok(new OkResponse("Successfully deleted")) 
            : TypedResults.InternalServerError(new ServerErrorResponse(result.Messages));
    }
}