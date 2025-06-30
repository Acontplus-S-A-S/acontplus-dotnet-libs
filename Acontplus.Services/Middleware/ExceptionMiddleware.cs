namespace Acontplus.Services.Middleware;

public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var requestBody = string.Empty;

        try
        {
            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                requestBody = await ReadRequestBodyAsync(context);
            }
            await next(context);
        }
        catch (Exception ex)
        {
            var queryString = context.Request.QueryString.ToString();

            logger.LogError(ex, "Exception caught: {Message}. Request Body: {RequestBody}. Query String: {QueryString}",
                ex.Message, requestBody, queryString);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var stackTrace = ex.StackTrace?.Replace(Environment.NewLine, "\n");

            var problemDetails = env.IsDevelopment()
                ? new ProblemDetails
                {
                    Status = context.Response.StatusCode,
                    Title = "Server Error: " + ex.Message,
                    Detail = stackTrace
                } : new ProblemDetails
                {
                    Status = context.Response.StatusCode,
                    Title = "Server Error: " + ex.Message,
                };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
    private async Task<string> ReadRequestBodyAsync(HttpContext context)
    {
        var request = context.Request;
        var requestContent = "";
        request.EnableBuffering();
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
        {
            requestContent = await reader.ReadToEndAsync();
        }
        request.Body.Position = 0; // Rewind the stream to 0

        return requestContent;
    }
}
