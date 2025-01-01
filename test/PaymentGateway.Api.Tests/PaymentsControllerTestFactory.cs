using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Abstraction;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTestFactory
{
    public static HttpClient GetWebClient(PaymentsRepository? paymentsRepository = null, bool useRealPaymentProcessor = false)
    {
        var repo = paymentsRepository ?? new PaymentsRepository();
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var paymentProcessorMock = new Mock<IPaymentProcessor>();

        var client = webApplicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IPaymentsRepository>(repo); // Add the payments repository
                    services.AddSingleton<IBank, SimulatorBank>();

                    if (useRealPaymentProcessor)
                    {
                        services.AddSingleton<IPaymentProcessor, PaymentProcessorService>();
                    }
                    else
                    {
                        services.AddSingleton(paymentProcessorMock.Object);
                    }
                }))
            .CreateClient();
        return client;
    }
}