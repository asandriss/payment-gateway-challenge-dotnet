using LanguageExt;
using PaymentGateway.Abstraction;
using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Services
{
    public class PaymentsRepository : IPaymentsRepository
    {
        public List<PostPaymentResponse> Payments = new();

        public void Add(PostPaymentResponse payment)
        {
            Payments.Add(payment);
        }

        public async Task<Option<PostPaymentResponse>> GetAsync(Guid id)
        {
            var payment = Payments.FirstOrDefault(p => p.Id == id);
            var result = payment
                         ?? Option<PostPaymentResponse>.None;

            return await Task.FromResult(result);
        }
    }
}
