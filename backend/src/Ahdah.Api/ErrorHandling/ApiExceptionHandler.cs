using Microsoft.AspNetCore.Diagnostics;

namespace Ahdah.Api.ErrorHandling;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            "Unhandled API exception of type {ExceptionType}. Trace identifier: {TraceIdentifier}",
            exception.GetType().Name,
            httpContext.TraceIdentifier);

        await Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unexpected server error",
                detail: "An unexpected error occurred.",
                extensions: new Dictionary<string, object?>
                {
                    ["code"] = "server.unexpected_error",
                    ["traceId"] = httpContext.TraceIdentifier
                })
            .ExecuteAsync(httpContext);

        return true;
    }
}
