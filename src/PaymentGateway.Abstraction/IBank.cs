using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Abstraction;

public interface IBank
{
    public Task<BankCardResponse> ProcessCreditCardPayment(BankCardRequest request);
}