using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

using Newtonsoft.Json;

using PaymentGateway.Abstraction.Models;
using PaymentGateway.Api.Controllers;

namespace PaymentGateway.Api.Tests
{
    public class PostPaymentEndpointTests
    {
        [Fact]
        public async Task Returns401IfNotAuthorized()
        {
            // Arrange
            var client = PaymentsControllerTestFactory.GetWebClient();
            StringContent content = GetWorkingCardRequestContent();

            // Act
            var response = await client.PostAsync($"/api/Payments/", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private static StringContent GetWorkingCardRequestContent()
        {
            var request = new PostPaymentRequest
            {
                RequestId = Guid.NewGuid(),
                CardNumber = 4111111111111111,
                ExpiryMonth = 12,
                ExpiryYear = 2025,
                Currency = "USD",
                Amount = 100,
                Cvv = 123
            };
            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            return content;
        }
    }
}
