using BuildingBlocks;

using FluentAssertions;

using PaymentGateway.Api.Services;
using PaymentGateway.Services;

namespace PaymentGateway.Api.Tests.Services
{
    public class CardValidatorTests
    {
        private readonly CardValidator _validator = new CardValidator(new CurrencyProvider());

        [Theory]
        [InlineData(2222405343248877, true)]
        [InlineData(1234567890, false)]
        [InlineData(0, false)]
        [InlineData(2222405343248112, false)]        // without Luhn check this returns true
        [InlineData(-2222405343248877, false)]
        [InlineData(long.MinValue, false)]
        [InlineData(long.MaxValue, false)]           // maximum number of characters is 19, without Luhn check this would return true
        public void TestCardValidationRules(long input, bool expected)
        {
            var result = _validator.ValidateCardNumber(input).Match(
                card => true, 
                err => false);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(111, true)]
        [InlineData(1111, true)]
        [InlineData(9999, true)]
        [InlineData(99, false)]
        [InlineData(19999, false)]
        [InlineData(0, false)]
        [InlineData(-111, false)]
        [InlineData(-9999, false)]
        public void TestCvvLengthRules(int cvv, bool expected)
        {
            var result = _validator.ValidateCvv(cvv).IsSuccess;

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(11, true)]
        [InlineData(12, true)]
        [InlineData(13, false)]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        public void TestExpiryMonth(int month, bool expected)
        {
            var result = _validator.ValidateExpirationMonth(month).IsSuccess;

            result.Should().Be(expected);
        }

        [Fact]
        public void TestExpiryYearValidation()
        {
            _validator.ValidateExpirationYear(DateTime.UtcNow.Year).IsSuccess.Should().BeTrue();
            _validator.ValidateExpirationYear(DateTime.UtcNow.Year + 1).IsSuccess.Should().BeTrue();
            
            _validator.ValidateExpirationYear(DateTime.UtcNow.Year - 1).IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void TestExpiryDateValidation()
        {
            SystemDateTime.ForceDateTimeProvider(() => new DateTime(2024, 11, 30, 12, 0, 0 ));

            _validator.ValidateExpirationDate(11, 2024).IsSuccess.Should().BeTrue();        // debatable - we need to clarify requirements "should be in the future". If it's in the current month should still be valid.
            _validator.ValidateExpirationDate(12, 2024).IsSuccess.Should().BeTrue();
            _validator.ValidateExpirationDate(1, 2025).IsSuccess.Should().BeTrue();
            _validator.ValidateExpirationDate(10, 2025).IsSuccess.Should().BeTrue();
            _validator.ValidateExpirationDate(7, 2050).IsSuccess.Should().BeTrue();         // not sure if there should be a max expiry duration?

            _validator.ValidateExpirationDate(0, 2024).IsSuccess.Should().BeFalse();
            _validator.ValidateExpirationDate(10, 0).IsSuccess.Should().BeFalse();
            _validator.ValidateExpirationDate(10, 2024).IsSuccess.Should().BeFalse();
            _validator.ValidateExpirationDate(11, 2023).IsSuccess.Should().BeFalse();
            _validator.ValidateExpirationDate(12, 2023).IsSuccess.Should().BeFalse();

            SystemDateTime.Reset();
        }

        [Theory]
        [InlineData("USD", true)]
        [InlineData("EUR", true)]
        [InlineData("GBP", true)]
        [InlineData("gbp", true)]
        [InlineData("GP", false)]
        [InlineData("$", false)]
        [InlineData("1223", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void TestCurrencyValidator(string? currency, bool expected)
        {
            _validator.ValidateCurrency(currency).IsSuccess.Should().Be(expected);
        }
    }
}