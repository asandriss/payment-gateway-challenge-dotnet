using LanguageExt;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Abstraction;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(PaymentsRepository paymentsRepository, IPaymentProcessor paymentProcessor) : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = await paymentsRepository.GetAsync(id);

        return payment.Match<ActionResult>(
            Some: p => new OkObjectResult(p),
            None: () => new NotFoundObjectResult(id));

    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> ProcessPaymentAsync([FromBody] PostPaymentRequest request)
    {
        var cardValidator = new CardValidator(new List<string>() { "GBP", "USD", "EUR" });

        var validationResult = cardValidator.ValidateRequest(request);

        if (validationResult.IsFail)
            // ToDo: Add logging here
            return new BadRequestObjectResult(validationResult.Head());         // here we're just returning the first error encountered

        var validCard = validationResult.Match(
            Succ: x => x,
            Fail: _ => throw new InvalidOperationException("this should not be reached..."));

        var bankRequest = new PaymentProcessorRequest
        (
            Amount: validCard.Amount,
            CardNumber: validCard.CardNumber.ToString(),
            Currency: validCard.Currency,
            ExpiryDate: validCard.GetExpiryString(),
            Cvv: validCard.Cvv.ToString()
        );

        var result = await paymentProcessor.ProcessPayment(bankRequest);

        return new OkObjectResult(result);
    }
}