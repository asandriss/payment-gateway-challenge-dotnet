using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mapster;

using PaymentGateway.Abstraction;
using PaymentGateway.Abstraction.Enum;
using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Services
{
    public class PaymentProcessorService : IPaymentProcessor
    {
        public Task<PaymentProcessorResponse> ProcessPayment(PaymentProcessorRequest request)
        {
            var result = new PaymentProcessorResponse(PaymentStatus.Rejected, Guid.NewGuid(), request.RequestId);

            return Task.FromResult(result);
        }
    }
}
