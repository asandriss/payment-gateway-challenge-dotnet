using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Extensions;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(PaymentsRepository paymentsRepository) : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = paymentsRepository.Get(id);

        return payment != null ? new OkObjectResult(payment) : new NotFoundObjectResult(id);
    }

    public async Task<ActionResult<PostPaymentResponse>> ProcessPaymentAsync(PostPaymentRequest request)
    {
        var payment = new PostPaymentResponse()
        {
            Amount = request.Amount,
            CardNumberLastFour = request.CardNumber.GetLastFourDigits(),
            Currency = request.Currency,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear
        };

        throw new NotImplementedException();
    }
}