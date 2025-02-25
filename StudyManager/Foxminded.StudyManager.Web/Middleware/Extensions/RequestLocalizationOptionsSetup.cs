using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Foxminded.StudyManager.Web.Middleware.Extensions;

public static class RequestLocalizationOptionsSetup
{
    public static IServiceCollection GetRequestLocalizationOptions(this IServiceCollection services)
    {
        var supportedCultures = new[]
        {
            new CultureInfo("uk-UA"),
            new CultureInfo("en-US")
        };

        return services.Configure<RequestLocalizationOptions>(options =>
         {
             options.DefaultRequestCulture = new RequestCulture("uk-UA");
             options.SupportedCultures = supportedCultures;
             options.SupportedUICultures = supportedCultures;
         });
    }
}
