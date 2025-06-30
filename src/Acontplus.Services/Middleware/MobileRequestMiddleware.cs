namespace Acontplus.Services.Middleware
{
    public class MobileRequestMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var isMobileUserAgent = IsMobileUserAgent(userAgent);

            var fromMobile = context.Request.Headers["X-Is-Mobile"].ToString()
                .Equals("true", StringComparison.OrdinalIgnoreCase);

            context.Items["FromMobile"] = fromMobile;
            context.Items["IsMobileUserAgent"] = isMobileUserAgent;

            await next(context);
        }

        private static bool IsMobileUserAgent(string userAgent)
        {
            var mobileKeywords = new[]
            {
                "Android", "iPhone", "iPad", "iPod", "Windows Phone", "IEMobile", "BlackBerry", "BB10",
                "Opera Mini", "Mobile", "Silk"
            };

            return mobileKeywords.Any(keyword => userAgent.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }
    }
}
