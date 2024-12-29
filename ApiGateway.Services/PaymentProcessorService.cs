using BuildingBlocks;

using LanguageExt;

using Mapster;

using PaymentGateway.Abstraction;
using PaymentGateway.Abstraction.Enum;
using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Services;

public class PaymentProcessorService : IPaymentProcessor
{
    private readonly IBank _bankService;
    private readonly IPaymentsRepository _paymentDb;

    public PaymentProcessorService(IBank bankService, IPaymentsRepository paymentDb)
    {
        _bankService = bankService;
        _paymentDb = paymentDb;
        
        ConfigureMappings();
    }
    
    public async Task<Either<PaymentProcessorResponse, string>> ProcessPayment(PaymentProcessorRequest request)
    {
        var cardValidator = new CardValidator(new CurrencyProvider());

        var validationResult = cardValidator.ValidateRequest(request);

        if (validationResult.IsFail)
        {
            var allErrors = validationResult.Match(
                Fail: err => err.AsIterable().Select(e => e.ToString()).ToArray(),
                Succ: _ => []
            );

            // ToDo: Log all errors here
            // ToDo: Write the failed request into DB
            return string.Join("; ", allErrors);
        }
        
        var bankRequest = request.Adapt<BankCardRequest>();
        var bankResponse = await _bankService.ProcessCreditCardPayment(bankRequest);

        WriteResultsToTheDb(bankResponse, request, _paymentDb);

        var result = new PaymentProcessorResponse(PaymentStatus.Rejected, Guid.NewGuid(), request.RequestId);

        return result;
    }

    private void WriteResultsToTheDb(BankCardResponse bankResponse, PaymentProcessorRequest request,
        IPaymentsRepository paymentsRepository)
    {
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
    
    private static void ConfigureMappings()
    {
        TypeAdapterConfig.GlobalSettings.ForType<PaymentProcessorRequest, BankCardRequest>()
            .Map(dest => dest.ExpiryDate, src => $"{src.ExpiryMonth:D2}/{src.ExpiryYear}");
    }

}