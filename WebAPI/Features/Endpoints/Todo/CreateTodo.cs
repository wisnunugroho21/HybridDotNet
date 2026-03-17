using General.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common.API;

namespace WebAPI.Features.Endpoints.Todo;

public class CreateTodo : IEndpoint
{
    public record Request(General.Data.Types.Todo Todo);

    public record OkResponse(string Message);
    
    public record ServerErrorResponse(IEnumerable<string>? Messages);

    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithName(nameof(CreateTodo))
        .WithSummary("Create a new Todo");

    private static async Task<Results<Ok<OkResponse>, InternalServerError<ServerErrorResponse>>> Handle([FromBody] Request request, [FromServices] TodoService service,
        CancellationToken cancellationToken)
    {
        var result = await service.Create(request.Todo, cancellationToken);

        return result.IsSuccess 
            ? TypedResults.Ok(new OkResponse("Successfully created")) 
            : TypedResults.InternalServerError(new ServerErrorResponse(result.Messages));
    }
}