using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Api.Extensions;

public static class PaymentGatewayExtensions
{

    public static string GetExpiryString(this PostPaymentRequest request) =>
        $"{request.ExpiryMonth}/{request.ExpiryYear}";

}