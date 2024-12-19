namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest
{
    public long CardNumber { get; set; }            // Maybe we should pass it as a string?
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string? Currency { get; set; }
    public int Amount { get; set; }                 // there's a requirement for the amount to be integer, so I'm keeping it.
                                                    //   But I would recommend amount to be long (or a string) due to some currencies having very large values
    public int Cvv { get; set; }
}