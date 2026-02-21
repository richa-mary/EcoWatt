namespace EcoWattAPI.Services
{
    public record QuoteResult(decimal EstimatedMonthlyAmount, decimal EstimatedAnnualAmount, int TariffId, string TariffName);

    public interface IQuoteService
    {
        Task<QuoteResult?> CalculateQuoteForCustomerAsync(int customerId, decimal? expectedMonthlyElectricity = null, decimal? expectedMonthlyGas = null, CancellationToken ct = default);
        Task<QuoteResult?> CalculateQuoteForTariffAsync(int tariffId, decimal monthlyElectricity, decimal monthlyGas, CancellationToken ct = default);
    }
}