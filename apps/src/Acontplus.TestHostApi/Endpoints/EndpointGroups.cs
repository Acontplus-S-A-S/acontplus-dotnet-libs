using Acontplus.TestHostApi.Endpoints.Customer;

namespace Acontplus.TestHostApi.Endpoints;

public static class EndpointGroups
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapIdentificationEndpoints();
    }
}