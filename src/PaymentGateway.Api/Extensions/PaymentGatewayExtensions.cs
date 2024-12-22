using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Api.Extensions;

public static class PaymentGatewayExtensions
{
    public static int GetLastFourDigits(this long value) => (int)(value % 10_000);

    public static string GetExpiryString(this PostPaymentRequest request) =>
        $"{request.ExpiryMonth}/{request.ExpiryYear}";

}