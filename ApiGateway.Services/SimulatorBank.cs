using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PaymentGateway.Abstraction;

namespace PaymentGateway.Services
{
    public class SimulatorBank : IBank
    {
        public Task<BankCardResponse> ProcessCreditCardPayment(BankCardRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
