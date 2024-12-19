namespace PaymentGateway.Api.Extensions
{
    public static class PaymentGatewayExtensions
    {
        public static int GetLastFourDigits(this long value) => (int)(value % 10_000);

    }
}
