using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Abstraction
{
    public interface IPaymentProcessor
    {
        Task<PaymentProcessorResponse> ProcessPayment(PaymentProcessorRequest request);
    }
}
