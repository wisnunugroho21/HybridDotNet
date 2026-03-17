using General.Common.Query;
using General.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebAPI.Common.API;

namespace WebAPI.Features.Endpoints.Todo;

public class GetAllTodo : IEndpoint
{
    public record Request(int Skip = 0, int Take = 0, string Filters = "[]", string Sorts = "[]");
    
    public record OkResponse(IEnumerable<General.Data.Types.Todo> Data, int Total);
    
    public record ServerErrorResponse(IEnumerable<string>? Messages);

    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithName(nameof(GetAllTodo))
        .WithSummary("Create a new Todo");

    private static async Task<Results<Ok<OkResponse>, InternalServerError<ServerErrorResponse>>> Handle([AsParameters] Request request, [FromServices] TodoService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetAll(request.Skip, request.Take, 
            JsonConvert.DeserializeObject<List<Filter>>(
                !string.IsNullOrWhiteSpace(request.Filters) ? request.Filters : "[]"
            ) ?? [], 
            JsonConvert.DeserializeObject<List<Sort>>(
                !string.IsNullOrWhiteSpace(request.Sorts) ? request.Sorts : "[]"
            ) ?? [], 
            cancellationToken);
        
        return result.Errors is not null && result.Errors.Any()
            ? TypedResults.InternalServerError(new ServerErrorResponse(result.Errors))
            : TypedResults.Ok(new OkResponse(result.Data, result.Total));
    }
}