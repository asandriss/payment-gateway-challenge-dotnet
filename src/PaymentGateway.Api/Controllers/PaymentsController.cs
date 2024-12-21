using Mapster;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Abstraction;
using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Services;
using PaymentGateway.Services;
using PaymentGateway.Abstraction.Models;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentsRepository paymentsRepository, IPaymentProcessor paymentProcessor) : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = await paymentsRepository.GetAsync(id);

        return payment.Match<ActionResult>(
            Some: p => new OkObjectResult(p),
            None: () => new NotFoundObjectResult(id));
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> ProcessPaymentAsync([FromBody] PostPaymentRequest request)
    {
        var cardValidator = new CardValidator(new CurrencyProvider());

        var validationResult = cardValidator.ValidateRequest(request);

        if (validationResult.IsFail)
        {
            var allErrors = validationResult.Match(
                Fail: err => string.Join("; ", err.AsIterable().Select(e => e.ToString())),
                Succ: _ => string.Empty
            );
        
            // ToDo: Log all errors here
            return new BadRequestObjectResult(allErrors);
        }

        var validCard = validationResult.Match(
            Succ: x => x,
            Fail: _ => throw new InvalidOperationException("this should not be reached..."));

        var bankRequest = request.Adapt<PaymentProcessorRequest>();

        var processingResult = await paymentProcessor.ProcessPayment(bankRequest);


        var result = new PostPaymentResponse()
        {
            Amount = validCard.Amount,
            RequestId = request.RequestId,
            CardNumberLastFour = validCard.CardNumber.GetLastFourDigits(),
            Currency = validCard.Currency,
            ExpiryMonth = validCard.ExpiryMonth,
            ExpiryYear = validCard.ExpiryYear,
            Id = Guid.NewGuid(),
            Status = processingResult.Status
        };


        return new OkObjectResult(result);
    }
}