using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LanguageExt;

using PaymentGateway.Abstraction.Enum;

namespace PaymentGateway.Abstraction.Models
{
    public record PaymentProcessorResponse(
        PaymentStatus Status,
        Guid Id,
        Guid RequestId);
}
