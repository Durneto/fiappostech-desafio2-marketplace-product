namespace ProductApi.Helpers.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // Log the request details
            _logger.LogInformation($"Request: {httpContext.Request.Method} {httpContext.Request.Path}");

            // Log the request body
            httpContext.Request.EnableBuffering();
            var requestBodyStream = new MemoryStream();
            await httpContext.Request.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            var requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
            _logger.LogInformation($"Request Body: {requestBodyText}");

            // Capture the response
            var originalBodyStream = httpContext.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                httpContext.Response.Body = responseBody;

                await _next(httpContext);

                // Log the response details
                _logger.LogInformation($"Response: {httpContext.Response.StatusCode}");

                // Log the response body
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
                _logger.LogInformation($"Response Body: {responseBodyText}");

                // Copy the response body to the original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingApi(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
