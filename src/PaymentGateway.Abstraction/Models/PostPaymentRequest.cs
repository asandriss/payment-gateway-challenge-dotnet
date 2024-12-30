namespace PaymentGateway.Abstraction.Models;

public record PostPaymentRequest
{
    public required Guid RequestId { get; init; }             // The ID here will be used at least for the client to be able to co-relate requests in their logs as well as to make sure no duplicate transactions are processed
    public required long CardNumber { get; init; }            // Maybe we should pass it as a string?
    public required int ExpiryMonth { get; init; }
    public required int ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }                 // there's a requirement for the amount to be integer, so I'm keeping it.
                                                             //   But I would recommend amount to be long (or a string) due to some currencies having very large values
    public required int Cvv { get; init; }
}