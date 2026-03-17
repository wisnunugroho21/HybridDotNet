using General.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common.API;

namespace WebAPI.Features.Endpoints.Todo;

public class UpdateTodo : IEndpoint
{
    public record Request(General.Data.Types.Todo Todo);

    public record OkResponse(string Message);
    
    public record ServerErrorResponse(IEnumerable<string>? Messages);

    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/", Handle)
        .WithName(nameof(UpdateTodo))
        .WithSummary("Create a new Todo");

    private static async Task<Results<Ok<OkResponse>, InternalServerError<ServerErrorResponse>>> Handle([FromBody] Request request, [FromServices] TodoService service,
        CancellationToken cancellationToken)
    {
        var result = await service.Update(request.Todo, cancellationToken);

        return result.IsSuccess 
            ? TypedResults.Ok(new OkResponse("Successfully updated")) 
            : TypedResults.InternalServerError(new ServerErrorResponse(result.Messages));
    }
}