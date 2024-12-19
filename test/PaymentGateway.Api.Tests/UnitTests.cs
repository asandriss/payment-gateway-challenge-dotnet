using FluentAssertions;

using PaymentGateway.Api.Extensions;

namespace PaymentGateway.Api.Tests
{
    public class UnitTests
    {
        public class ExtensionTests
        {
            [Theory]
            [InlineData(1234567890123456789, 6789)]
            [InlineData(2222405343248877, 8877)]
            [InlineData(2222405343248112, 8112)]
            public void GetLastFourDigitsTest(long cardNum, int expected)
            {
                cardNum.GetLastFourDigits().Should().Be(expected);
            }
        }
    }
}
