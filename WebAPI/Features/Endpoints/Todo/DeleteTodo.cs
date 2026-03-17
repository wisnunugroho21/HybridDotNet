using General.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common.API;

namespace WebAPI.Features.Endpoints.Todo;

public class DeleteTodo : IEndpoint
{
    public record Request(long Id);

    public record OkResponse(string Message);
    
    public record ServerErrorResponse(IEnumerable<string>? Messages);

    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{Id:long}", Handle)
        .WithName(nameof(DeleteTodo))
        .WithSummary("Delete a Todo");

    private static async Task<Results<Ok<OkResponse>, InternalServerError<ServerErrorResponse>>> Handle([AsParameters] Request request, [FromServices] TodoService service,
        CancellationToken cancellationToken)
    {
        var result = await service.Delete(request.Id, cancellationToken);
        
        return result.IsSuccess 
            ? TypedResults.Ok(new OkResponse("Successfully deleted")) 
            : TypedResults.InternalServerError(new ServerErrorResponse(result.Messages));
    }
}