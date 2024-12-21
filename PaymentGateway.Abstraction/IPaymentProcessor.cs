using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Abstraction
{
    public interface IPaymentProcessor
    {
        Task<PaymentProcessorResponse> ProcessPayment(PaymentProcessorRequest request);
    }
}
