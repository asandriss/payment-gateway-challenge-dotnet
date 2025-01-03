﻿using System.Net;
using System.Text;
using BuildingBlocks;
using FluentAssertions;
using Newtonsoft.Json;
using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Api.Tests;

public class PostPaymentEndpointTests
{
    [Fact]
    public async Task Returns401IfNotAuthorized()
    {
        // Arrange
        var client = PaymentsControllerTestFactory.GetWebClient();
        StringContent content = GetWorkingCardRequestContent();

        // Act
        var response = await client.PostAsync("/api/Payments/", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CompletesASuccessfulPayment_INTEGRATION_RequiresBankSimulator()
    {
        // Arrange
        var client = PaymentsControllerTestFactory.GetWebClient(useRealPaymentProcessor: true);
        client.DefaultRequestHeaders.Add("Authorization", "key-123456");
        StringContent content = GetWorkingCardRequestContent();

        // Act
        var response = await client.PostAsync("/api/Payments/", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var paymentResponse = JsonConvert.DeserializeObject<GetPaymentResponse>(responseContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentResponse.Should().NotBeNull();
    }

    [Theory, MemberData(nameof(PostPaymentValidationTestSuite))]
    public async Task TestValidationRules(PostPaymentRequest request, string expectedMessage)
    {
        // Arrange
        SystemDateTime.ForceDateTimeProvider(() => new DateTime(2024, 11, 30, 12, 0, 0));

        var client = PaymentsControllerTestFactory.GetWebClient(useRealPaymentProcessor: true);
        client.DefaultRequestHeaders.Add("Authorization", "key-123456");
            
        var jsonRequest = JsonConvert.SerializeObject(request);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/Payments/", content);
        var paymentResponse = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        paymentResponse.Should().NotBeNull();
        paymentResponse!.Should().Be(expectedMessage);

        SystemDateTime.Reset();
    }

    private static StringContent GetWorkingCardRequestContent()
    {
        var request = DefaultRequest;
        var jsonRequest = JsonConvert.SerializeObject(request);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            
        return content;
    }

    private static readonly PostPaymentRequest DefaultRequest = new()
    {
        RequestId = Guid.NewGuid(),
        CardNumber = 2222405343248877,
        ExpiryMonth = 4,
        ExpiryYear = 2025,
        Currency = "GBP",
        Amount = 100,
        Cvv = 123
    };
        
    public static IEnumerable<object[]> PostPaymentValidationTestSuite()
    {
        yield return
        [
            DefaultRequest with {Amount = 0},
            "Amount must be greater than zero. Was: [0]"
        ];
        yield return
        [
            DefaultRequest with {Amount = -1},
            "Amount must be greater than zero. Was: [-1]"
        ];
        yield return
        [
            DefaultRequest with {CardNumber = 0},
            "Credit card number must be between 14 and 19 characters long. Was: [1] characters long"
        ];
        yield return
        [
            DefaultRequest with {CardNumber = -2222405343248877},
            "Negative card number provided: [-2222405343248877]"
        ];
        yield return
        [
            DefaultRequest with {CardNumber = 2222405343248870},
            "Luhn checksum check failed for card [2222405343248870]"
        ];
        yield return
        [
            DefaultRequest with {Currency = "JPY"},
            "Unsupported currency [JPY]"
        ];
        yield return
        [
            DefaultRequest with {ExpiryYear = 2024, ExpiryMonth = 9},
            "Card expired"
        ];
        yield return
        [
            DefaultRequest with {Cvv = 1},
            "CVV must be between 3 and 4 characters long. Was: [1]"
        ];
        yield return
        [
            DefaultRequest with {Cvv = 12345},
            "CVV must be between 3 and 4 characters long. Was: [12345]"
        ];
    }
}