using LanguageExt;

namespace PaymentGateway.Abstraction;

public interface IApiKeysRepository
{
    Option<string> GetMerchantId(string apiKey);
}