using BuildingBlocks;

using Mapster;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Abstraction;
using PaymentGateway.Abstraction.Models;
using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentsRepository paymentsRepository, IPaymentProcessor paymentProcessor, ILogger<PaymentsController> logger) : Controller
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        logger.LogInformation("GET Request received at GetPaymentAsync. Request data payment for GUID {id}", id);
        var payment = await paymentsRepository.GetAsync(id);

        logger.LogInformation("Results ready for request {id}. Payment {payment}", id, payment);

        return payment.Match<ActionResult>(
            Some: p => new OkObjectResult(p),
            None: () => new NotFoundObjectResult(id));
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> ProcessPaymentAsync([FromBody] PostPaymentRequest request)
    {
        logger.LogInformation("POST request received at ProcessPaymentAsync. Request data: {request}", request);
        var processingResult = await paymentProcessor.ProcessPayment(request.Adapt<PaymentProcessorRequest>());

        var result = processingResult.Match<ActionResult>(
            Left: response => new OkObjectResult( new PostPaymentResponse
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
            Right: err => new BadRequestObjectResult(err)
            );

        logger.LogInformation("Response from ProcessPaymentAsync ready. Result: {result}", result);

        return result;
    }
}