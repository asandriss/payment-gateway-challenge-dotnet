using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Abstraction.Models
{
    public record PaymentProcessorRequest(
        Guid RequestId,
        long CardNumber,
        int ExpiryMonth,
        int ExpiryYear,
        string Currency,
        int Amount,
        int Cvv
    );
}
