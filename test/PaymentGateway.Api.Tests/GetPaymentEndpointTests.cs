﻿using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Abstraction;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using Moq;

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

        var client = GetWebClient(paymentsRepository);
        client.DefaultRequestHeaders.Add("Authorization", "key-123456");

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
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

        HttpClient client = GetWebClient(paymentsRepository);
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

    private static HttpClient GetWebClient(PaymentsRepository paymentsRepository)
    {
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        //var client = webApplicationFactory.CreateClient();
        var paymentProcessorMock = new Mock<IPaymentProcessor>();


        var client = webApplicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(paymentsRepository); // Add the payments repository
                    services.AddSingleton(paymentProcessorMock.Object); // Add the mocked payment processor
                }))
            .CreateClient();
        return client;
    }

}