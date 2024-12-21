using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Abstraction
{
    public record PaymentProcessorRequest(
        string CardNumber,
        string ExpiryDate,
        string Currency,
        int Amount,
        string Cvv)
    {
    }
}
