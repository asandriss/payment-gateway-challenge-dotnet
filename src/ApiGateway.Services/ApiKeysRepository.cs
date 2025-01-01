using LanguageExt;

using PaymentGateway.Abstraction;

namespace PaymentGateway.Services;

public class ApiKeysRepository : IApiKeysRepository
{
    private readonly Dictionary<string, string> _apiKeys = new()
    {
        { "key-123456", "merchant1" },
        { "key-789012", "merchant2" }
    };

    public Option<string> GetMerchantId(string apiKey)
    {
        return _apiKeys.TryGetValue(apiKey, out var merchant)
            ? Option<string>.Some(merchant)
            : Option<string>.None;
    }
}