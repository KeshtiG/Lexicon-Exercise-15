using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Tournament.Api.Extensions;

public static class ExceptionMiddleware
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var contextFeatures = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeatures != null)
                {
                    var problemDetailsFactory = app.Services.GetServices<ProblemDetailsFactory>();
                    ArgumentNullException.ThrowIfNull(nameof(problemDetailsFactory));
                }

            });
        });
    }
}
