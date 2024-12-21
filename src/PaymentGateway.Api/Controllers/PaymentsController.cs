using Mapster;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Abstraction;
using PaymentGateway.Abstraction.Models;
using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Services;

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
        var processingResult = await paymentProcessor.ProcessPayment(request.Adapt<PaymentProcessorRequest>());

        var result = processingResult.Match<ActionResult>(
            response => new OkObjectResult( new PostPaymentResponse
                {
                    RequestId = response.RequestId,
                    Id = response.Id,
                    Status = response.Status,
                    Amount = request.Amount,
                    ExpiryMonth = request.ExpiryMonth,
                    ExpiryYear = request.ExpiryYear,
                    CardNumberLastFour = request.CardNumber.GetLastFourDigits(),
                    Currency = request.Currency
                    
                }),
            err => new BadRequestObjectResult(err)
            );

        return result;
    }
}