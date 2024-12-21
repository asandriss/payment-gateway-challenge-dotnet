using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;

namespace PaymentGateway.Abstraction
{
    public record PaymentProcessorResponse(
        bool Authorized,
        Option<string> AuthorizationCode,
        Guid RequestId);
}
