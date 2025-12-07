namespace Demo.Api.Endpoints;

public static class EndpointGroups
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapIdentificationEndpoints();
    }
}

