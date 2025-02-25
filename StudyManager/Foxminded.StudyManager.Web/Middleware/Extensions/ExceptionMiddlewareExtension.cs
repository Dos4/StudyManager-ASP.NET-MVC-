namespace Foxminded.StudyManager.Web.Middleware.Extensions;

public static class ExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseGlobalExceptionHandler(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
