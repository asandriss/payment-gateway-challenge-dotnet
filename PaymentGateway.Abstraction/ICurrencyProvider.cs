using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Abstraction
{
    public interface ICurrencyProvider
    {
        IList<string> GetSupportedCurrencies();
    }
}
