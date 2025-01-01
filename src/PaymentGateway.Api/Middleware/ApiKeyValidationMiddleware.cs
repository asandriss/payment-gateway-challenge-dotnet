using PaymentGateway.Abstraction;

namespace PaymentGateway.Api.Middleware;

public class ApiKeyValidationMiddleware(RequestDelegate next, IApiKeysRepository apiKeys)
{
    private const string ApiKeyHeaderName = "Authorization";

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

    var merchantResult = apiKeys.GetMerchantId(apiKey);

    await merchantResult.Match(
        async merchantId =>
        {
            context.Items["MerchantId"] = merchantId;
            await next(context);
        },
        async () =>
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized merchant.");
        });
}

}