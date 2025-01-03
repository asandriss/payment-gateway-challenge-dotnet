﻿using PaymentGateway.Abstraction.Enum;

namespace PaymentGateway.Abstraction.Models;

public record GetPaymentResponse
{
    public Guid Id { get; init; }
    public required Guid RequestId { get; init; }
    public PaymentStatus Status { get; init; }
    public int CardNumberLastFour { get; init; }
    public int ExpiryMonth { get; init; }
    public int ExpiryYear { get; init; }
    public required string Currency { get; init; }
    public int Amount { get; init; }
}