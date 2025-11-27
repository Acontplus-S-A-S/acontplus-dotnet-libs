namespace Acontplus.TestApi.Endpoints.Infrastructure;

using Acontplus.Persistence.Common;

public static class PrintEndpoints
{
    public static void MapPrintEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/print")
            .WithTags("Print");

        group.MapGet("/", async (IAdoRepository adoRepository, string json) =>
        {
            var parameters = new Dictionary<string, object>
            {
                { "userRoleId", 27 },
                { "json", SqlStringParam.Sanitize(json) },
                { "isMobileUserAgent", "false" }
            };
            //var response = await adoRepository.QuerySingleOrDefaultAsync<ApiResponse>("Config.Print_Get", parameters);

            return Results.Ok();
        });
    }
}
