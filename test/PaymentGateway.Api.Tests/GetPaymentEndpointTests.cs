using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

using FluentAssertions;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Abstraction.Enum;
using PaymentGateway.Abstraction.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Services;

namespace PaymentGateway.Api.Tests;

public class GetPaymentEndpointTests
{
    private readonly Random _random = new();

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PostPaymentResponse
        {
            RequestId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2025, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999),
            Currency = "GBP",
            Status = PaymentStatus.Authorized
        };

        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.Add(payment);

        var client = PaymentsControllerTestFactory.GetWebClient(paymentsRepository);
        client.DefaultRequestHeaders.Add("Authorization", "key-123456");

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentResponse.Should().NotBeNull();
        paymentResponse.Should().BeEquivalentTo(payment);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var paymentsRepository = new PaymentsRepository();

        HttpClient client = PaymentsControllerTestFactory.GetWebClient(paymentsRepository);
        client.DefaultRequestHeaders.Add("Authorization", "key-123456");

        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Returns403IfNotAuthorized()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();

        var client = webApplicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


}

