using Newtonsoft.Json;
using RecruitmentPortal.Services.IServices;
using RecruitmentPortal.Services.Sevices;

namespace RecruitmentPortal.Middlewares
{
    public class BlacklistTokenMiddleware
    {

        private readonly RequestDelegate _next;


        public BlacklistTokenMiddleware(RequestDelegate next)
        {
            _next = next;
          
        }

        public async Task InvokeAsync(HttpContext context, IToken tokenService)
        {
            try
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader != null && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();

                    if (!tokenService.IsTokenValid(token))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token is invalid or blacklisted.");
                        return;
                    }
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);

            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "An error occurred while processing your request.",
                ExceptionMessage = ex.Message,
            };

            string jsonResponse = JsonConvert.SerializeObject(response);
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
