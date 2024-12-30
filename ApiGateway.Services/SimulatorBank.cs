using System.Text;
using System.Text.Json;

using PaymentGateway.Abstraction;
using PaymentGateway.Abstraction.Models;
using Microsoft.Extensions.Logging;

namespace PaymentGateway.Services
{
    public class SimulatorBank(HttpClient client, ILogger<SimulatorBank> logger) : IBank
    {
        public async Task<BankCardResponse> ProcessCreditCardPayment(BankCardRequest request)
        {
            logger.LogInformation("ProcessCreditCardPayment start with {request}", request);
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

                logger.LogInformation("Send request to the bank simulator");
                var httpResponse = await client.PostAsync(url, content);
                httpResponse.EnsureSuccessStatusCode();
                logger.LogInformation("Bank simulator returned success status code");

                var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                var bankResponse = JsonSerializer.Deserialize<BankCardResponse>(jsonResponse);

                logger.LogInformation("Bank returned the response {bankResponse}", bankResponse);
                return bankResponse ?? throw new Exception("Failed to deserialize bank response");
            }
            catch (Exception ex)
            {
                logger.LogError("Error occured while calling bank simulator {ex}", ex);
                throw;
            }
        }
    }
}
