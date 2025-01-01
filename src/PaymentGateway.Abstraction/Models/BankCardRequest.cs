namespace PaymentGateway.Abstraction.Models;

public record BankCardRequest(string CardNumber, string ExpiryDate, string Currency, int Amount, string Cvv);