using LanguageExt;

using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Abstraction;

public interface IPaymentProcessor
{
    Task<Either<PaymentProcessorResponse, string>> ProcessPayment(PaymentProcessorRequest request);
}