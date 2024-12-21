using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PaymentGateway.Abstraction;

namespace PaymentGateway.Services
{
    internal class PaymentProcessorService : IPaymentProcessor
    {
        public Task<PaymentProcessorResponse> ProcessPayment(PaymentProcessorRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
