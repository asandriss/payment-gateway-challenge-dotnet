using Microsoft.Extensions.Primitives;

using PaymentGateway.Abstraction;

namespace PaymentGateway.Api.Middleware;

public class ApiKeyValidationMiddleware(RequestDelegate next, IApiKeysRepository apiKeys)
{
    private const string ApiKeyHeaderName = "Authorization";
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey)
            || extractedApiKey.Count != 1
            || string.IsNullOrWhiteSpace(extractedApiKey[0]))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        var apiKey = extractedApiKey[0]!;

        _ = apiKeys.GetMerchantId(apiKey).Match(
            merchantId =>
            {
                context.Items["MerchantId"] = merchantId;
                _next(context);
            },
            () =>
            {
                context.Response.StatusCode = 401;
                context.Response.WriteAsync("Unauthorized merchant.");
                return;
            });
    }
}