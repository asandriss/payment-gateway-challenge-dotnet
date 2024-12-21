namespace PaymentGateway.Abstraction;

public record BankCardRequest(string CardNumber, string ExpiryDate, string Currency, int Amount, string Cvv);