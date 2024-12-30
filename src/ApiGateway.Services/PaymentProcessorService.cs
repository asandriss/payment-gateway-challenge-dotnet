using BuildingBlocks;
using LanguageExt;
using Mapster;

using Microsoft.Extensions.Logging;

using PaymentGateway.Abstraction;
using PaymentGateway.Abstraction.Enum;
using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Services;

public class PaymentProcessorService(
    IBank bankService,
    IPaymentsRepository paymentDb,
    ILogger<PaymentProcessorService> logger)
    : IPaymentProcessor
{
    public async Task<Either<PaymentProcessorResponse, string>> ProcessPayment(PaymentProcessorRequest request)
    {
        logger.LogInformation("PaymentProcessorService received a request: {request}", request);

        var cardValidator = new CardValidator(new CurrencyProvider());

        var validationResult = cardValidator.ValidateRequest(request);
        logger.LogInformation("PaymentProcessorService completed validation with: {validationResult}", validationResult);

        if (validationResult.IsFail)
        {
            var allErrors = validationResult.Match(
                Fail: err => err.AsIterable().Select(e => e.ToString()).ToArray(),
                Succ: _ => []
            );

            // This SHOULD NOT BE a warning. I made it so to stand out, should update it back to information.
            logger.LogWarning("Validation failed for request {id}. The following errors were found: {allErrors}", request.RequestId, allErrors);
            // ToDo: Write the failed request into DB
            return string.Join("; ", allErrors);
        }
        
        var bankRequest = request.Adapt<BankCardRequest>();
        logger.LogInformation("PaymentProcessorService is ready to call the bank service with request: {bankRequest}", bankRequest);
        var bankResponse = await bankService.ProcessCreditCardPayment(bankRequest);

        logger.LogInformation("PaymentProcessorService is ready to write data to the database: {bankResponse}", bankResponse);
        WriteResultsToTheDb(bankResponse, request, paymentDb);

        var result = new PaymentProcessorResponse(bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,  Guid.Parse(bankResponse.AuthorizationCode), request.RequestId);

        logger.LogInformation("PaymentProcessorService has completed with result {result}", result);
        return result;
    }

    private void WriteResultsToTheDb(BankCardResponse bankResponse, PaymentProcessorRequest request,
        IPaymentsRepository paymentsRepository)
    {
        logger.LogInformation("PaymentProcessorService writing data to the database: {request}, {bankResponse}", request, bankResponse);
        var payment = new PostPaymentResponse
        {
            Amount = request.Amount,
            CardNumberLastFour = request.CardNumber.GetLastFourDigits(),
            Currency = request.Currency,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Id = string.IsNullOrWhiteSpace(bankResponse.AuthorizationCode) ? Guid.Empty : Guid.Parse(bankResponse.AuthorizationCode),
            RequestId = request.RequestId,
            Status = bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
        };
        
        paymentsRepository.Add(payment);
    }
}