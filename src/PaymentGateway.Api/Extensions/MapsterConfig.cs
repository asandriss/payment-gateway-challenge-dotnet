using Mapster;

using PaymentGateway.Abstraction;
using PaymentGateway.Abstraction.Models;

namespace PaymentGateway.Api.Extensions
{
    public class MapsterConfig
    {
        private static bool s_isConfigured = false;
        private static readonly object Lock = new();

        public static void ConfigureMappings()
        {
            if (s_isConfigured) return;

            lock (Lock)
            {
                if (s_isConfigured) return;

                TypeAdapterConfig.GlobalSettings.ForType<PaymentProcessorRequest, BankCardRequest>()
                    .Map(dest => dest.ExpiryDate, src => $"{src.ExpiryMonth:D2}/{src.ExpiryYear}");

                s_isConfigured = true;
            }
        }
    }
}