using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Tournament.Core.Exeptions;

namespace Tournament.Api.Extensions
{
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
                        var problemDetailsFactory = app.Services.GetService<ProblemDetailsFactory>();
                        ArgumentNullException.ThrowIfNull(problemDetailsFactory, nameof(problemDetailsFactory));

                        var problemDetails = CreateProblemDetails(context, contextFeatures.Error, problemDetailsFactory, app);

                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

                        await context.Response.WriteAsJsonAsync(problemDetails);
                    }
                });
            });
        }

        private static ProblemDetails CreateProblemDetails(
            HttpContext context,
            Exception error,
            ProblemDetailsFactory problemDetailsFactory,
            WebApplication app)
        {
            return error switch
            {
                NotFoundException notFoundException => problemDetailsFactory.CreateProblemDetails(
                    context,
                    StatusCodes.Status404NotFound,
                    title: notFoundException.Title,
                    detail: $"{notFoundException.GetType().Name}: {notFoundException.Message}",
                    instance: context.Request.Path),

                GameLimitExceededException limitExceededException => problemDetailsFactory.CreateProblemDetails(
                    context,
                    StatusCodes.Status400BadRequest,
                    title: "Game Limit Exceeded",
                    detail: limitExceededException.Message,
                    instance: context.Request.Path),

                _ => problemDetailsFactory.CreateProblemDetails(
                    context,
                    StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error",
                    detail: app.Environment.IsDevelopment() ? error.Message : "An unexpected error occurred.")
            };
        }
    }
}

