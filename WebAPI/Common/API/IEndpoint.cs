namespace WebAPI.Common.API;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder builder);
}