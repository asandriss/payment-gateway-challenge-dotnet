using FluentAssertions;

using PaymentGateway.Abstraction;
using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests
{
    public class ExtensionTests
    {
        public static IEnumerable<object[]> GetTestPaymentRequest()
        {
            yield return
            [
                new PostPaymentRequest
                {
                    Amount = 0,
                    CardNumber = 0,
                    Currency = "",
                    Cvv = 0,
                    RequestId = Guid.Empty,
                    ExpiryMonth = 12,
                    ExpiryYear = 2025
                },
                "12/2025"
            ];
        }

        [Theory]
        [InlineData(1234567890123456789, 6789)]
        [InlineData(2222405343248877, 8877)]
        [InlineData(2222405343248112, 8112)]
        public void GetLastFourDigits_CorrectlySeparatesValues(long cardNum, int expected)
        {
            cardNum.GetLastFourDigits().Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(GetTestPaymentRequest))]
        public void GetExpiryString_MergesMonthAndYearCorrectly(PostPaymentRequest input, string expected)
        {
            input.GetExpiryString().Should().Be(expected);
        }
    }
}
