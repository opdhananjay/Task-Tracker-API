using devops.Helpers;
using Serilog;

namespace devops.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception ex)
            {
                await HandleException(httpContext, ex);
            }
        }

        public async Task HandleException(HttpContext httpContext,Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = 500;
            Log.Error(ex,"Error Occured.");
            await httpContext.Response.WriteAsJsonAsync(new ApiResponse<object>(false, 500, ex.Message, null));
        }
    }
}
