namespace PaymentGateway.Abstraction
{
    public interface ICurrencyProvider
    {
        IList<string> GetSupportedCurrencies();
    }
}
