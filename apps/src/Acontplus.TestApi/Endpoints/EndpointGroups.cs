namespace Acontplus.TestApi.Endpoints;

public static class EndpointGroups
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapIdentificationEndpoints();
        app.MapAdvancedEndpoints();
    }
}

