using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using LanguageExt.Traits.Domain;

using PaymentGateway.Abstraction;

namespace PaymentGateway.Services
{
    public class SimulatorBank(HttpClient client) : IBank
    {
        public async Task<BankCardResponse> ProcessCreditCardPayment(BankCardRequest request)
        {
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

                var httpResponse = await client.PostAsync(url, content);
                httpResponse.EnsureSuccessStatusCode();

                var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

                var bankResponse = JsonSerializer.Deserialize<BankCardResponse>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return bankResponse ?? throw new Exception("Failed to deserialize bank response");
            }
            catch (Exception ex)
            {
                // ToDo: Do proper logging here
                throw;
            }
        }
    }
}
