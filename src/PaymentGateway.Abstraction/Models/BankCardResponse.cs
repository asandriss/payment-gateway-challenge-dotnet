using System.Text.Json.Serialization;

namespace PaymentGateway.Abstraction.Models;

public record BankCardResponse(
    [property: JsonPropertyName("authorized")] bool Authorized,
    [property: JsonPropertyName("authorization_code")] string AuthorizationCode);