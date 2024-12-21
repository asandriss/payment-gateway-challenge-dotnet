using LanguageExt;

using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Abstraction
{
    public interface IPaymentProcessor
    {
        // Instead of Either, I would create a Result<> wrapper that would make it more readable
        Task<Either<PaymentProcessorResponse, string>> ProcessPayment(PaymentProcessorRequest request);
    }
}
