using PaymentGateway.Abstraction.Enum;

namespace PaymentGateway.Abstraction.Models;

public record PostPaymentResponse
{
    public Guid Id { get; init; }

    public required Guid RequestId { get; init; }
    public required PaymentStatus Status { get; init; }
    public required int CardNumberLastFour { get; init; }
    public required int ExpiryMonth { get; init; }
    public required int ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
}
