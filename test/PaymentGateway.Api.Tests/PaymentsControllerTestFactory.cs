using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Abstraction;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Services;
using PaymentGateway.Services;

namespace PaymentGateway.Api.Tests
{
    public class PaymentsControllerTestFactory
    {
        public static HttpClient GetWebClient(PaymentsRepository paymentsRepository = null)
        {
            var repo = paymentsRepository ?? new PaymentsRepository();
            var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
            //var client = webApplicationFactory.CreateClient();
            var paymentProcessorMock = new Mock<IPaymentProcessor>();


            var client = webApplicationFactory.WithWebHostBuilder(builder =>
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton<IPaymentsRepository>(repo); // Add the payments repository
                        services.AddSingleton(paymentProcessorMock.Object); // Add the mocked payment processor
                    }))
                .CreateClient();
            return client;
        }
    }
}
