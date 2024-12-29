namespace PaymentGateway.Abstraction.Models;

public record BankCardResponse(bool Authorized, string AuthorizationCode);