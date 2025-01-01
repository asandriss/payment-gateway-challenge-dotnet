using PaymentGateway.Abstraction.Enum;

namespace PaymentGateway.Abstraction.Models;

public record PaymentProcessorResponse(
    PaymentStatus Status,
    Guid Id,
    Guid RequestId);