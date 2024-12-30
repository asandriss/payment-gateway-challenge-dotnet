using LanguageExt;

using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Abstraction;

public interface IPaymentsRepository
{
    void Add(PostPaymentResponse payment);
        
    Task<Option<PostPaymentResponse>> GetAsync(Guid id);
}