using LanguageExt;

namespace PaymentGateway.Abstraction;

public record BankCardResponse(bool Authorized, Option<string> AuthorizationCode);