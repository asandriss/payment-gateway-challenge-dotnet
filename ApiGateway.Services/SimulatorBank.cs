using System.Text;
using System.Text.Json;

using PaymentGateway.Abstraction;
using PaymentGateway.Abstraction.Models;
using Microsoft.Extensions.Logging;

namespace PaymentGateway.Services
{
    public class SimulatorBank : IBank
    {
        private readonly HttpClient _client;
        private readonly ILogger<SimulatorBank> _logger;
        private static TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        public SimulatorBank(HttpClient client, ILogger<SimulatorBank> logger)
        {
            var handler = new HttpClientHandler() { AllowAutoRedirect = false };
            _client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(100),
            };

            _logger = logger;
        }

        public async Task<BankCardResponse> ProcessCreditCardPayment(BankCardRequest request)
        {
            _tcs.TrySetResult(false);
            _logger.LogInformation("ProcessCreditCardPayment start with {request}", request);
            const string url = "http://localhost:8080/payments";        // this should go out to config

            try
            {
                var jsonRequest = JsonSerializer.Serialize(new
                    {
                        card_number = request.CardNumber,
                        expiry_date = request.ExpiryDate,
                        currency = request.Currency,
                        amount = request.Amount,
                        cvv = request.Cvv
                    }
                );

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                _logger.LogInformation("Send request to the bank simulator");
                var httpResponse = await _client.PostAsync(url, content);
                httpResponse.EnsureSuccessStatusCode();
                _logger.LogInformation("Bank simulator returned success status code");

                var jsonResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                _tcs.TrySetResult(true);
                var bankResponse = JsonSerializer.Deserialize<BankCardResponse>(jsonResponse);

                _logger.LogInformation("Bank returned the response {bankResponse}", bankResponse);
                return bankResponse ?? throw new Exception("Failed to deserialize bank response");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while calling bank simulator {ex}", ex);
                throw;
            }
        }
    }
}
