using PaymentGateway.Abstraction;

namespace PaymentGateway.Services;

public class CurrencyProvider : ICurrencyProvider
{
    public IList<string> GetSupportedCurrencies()
    {
        return new List<string>() { "GBP", "USD", "EUR" };
    }
}