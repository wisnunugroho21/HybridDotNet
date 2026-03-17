using VeryMinimalAPI.Common.Auth.Endpoints;
using WebAPI.Features.Endpoints.Todo;
using WebAPI.Features.Endpoints.User;

namespace WebAPI.Common.API;

public static class EndpointService
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
        
        app.MapTodoEndpoints();
        app.MapUserEndpoints();
    }

    extension(IEndpointRouteBuilder app)
    {
        private void MapAuthEndpoints()
        {
            var endpoints = app
                .MapGroup("/auth")
                .WithTags("Auth")
                .AllowAnonymous();

            endpoints.MapEndpoint<Login>();
        }
        
        private void MapTodoEndpoints()
        {
            var endpoints = app
                .MapGroup("/todos")
                .WithTags("Todos")
                .RequireAuthorization();

            endpoints.MapEndpoint<GetAllTodo>()
                .MapEndpoint<GetTodo>()
                .MapEndpoint<CreateTodo>()
                .MapEndpoint<UpdateTodo>()
                .MapEndpoint<DeleteTodo>();
        }
        
        private void MapUserEndpoints()
        {
            var endpoints = app
                .MapGroup("/users")
                .WithTags("Users")
                .RequireAuthorization();

            endpoints.MapEndpoint<GetAllUser>()
                .MapEndpoint<GetUser>()
                .MapEndpoint<CreateUser>()
                .MapEndpoint<UpdateUser>()
                .MapEndpoint<DeleteUser>();
        }
        
        private IEndpointRouteBuilder MapEndpoint<T>()
            where T : IEndpoint
        {
            T.Map(app);
            return app;
        }
    }
}