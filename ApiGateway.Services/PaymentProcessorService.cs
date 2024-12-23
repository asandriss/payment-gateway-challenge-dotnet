﻿using LanguageExt;
using PaymentGateway.Abstraction;
using PaymentGateway.Abstraction.Enum;
using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Services;

public class PaymentProcessorService(IBank bankService) : IPaymentProcessor
{
    public Task<Either<PaymentProcessorResponse, string>> ProcessPayment(PaymentProcessorRequest request)
    {
        var cardValidator = new CardValidator(new CurrencyProvider());

        var validationResult = cardValidator.ValidateRequest(request);

        if (validationResult.IsFail)
        {
            var allErrors = validationResult.Match(
                Fail: err => err.AsIterable().Select(e => e.ToString()).ToArray(),
                Succ: _ => Array.Empty<string>()
            );

            // ToDo: Log all errors here
            return Task.FromResult<Either<PaymentProcessorResponse, string>>(string.Join("; ", allErrors));
        }

        var validCard = validationResult.Match(
            Succ: x => x,
            Fail: _ => throw new InvalidOperationException("this should not be reached..."));
        

        var result = new PaymentProcessorResponse(PaymentStatus.Rejected, Guid.NewGuid(), request.RequestId);

        return Task.FromResult<Either<PaymentProcessorResponse, string>>(result);
    }
}