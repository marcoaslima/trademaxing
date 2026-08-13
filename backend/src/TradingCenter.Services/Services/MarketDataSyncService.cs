using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Enums;
using TradingCenter.Domain.Interfaces;

namespace TradingCenter.Services.Services;

public class MarketDataSyncService : IMarketDataSyncService
{
    private readonly HttpClient _httpClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarketDataSyncService> _logger;

    public MarketDataSyncService(
        HttpClient httpClient,
        IUnitOfWork unitOfWork,
        ILogger<MarketDataSyncService> logger)
    {
        _httpClient = httpClient;
        _unitOfWork = unitOfWork;
        _logger = logger;

        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }
    }

    public async Task SyncAllMarketDataAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting complete market data synchronization process...");
        await SyncPtaxRatesAsync(DateTime.Today, ct);
        await SyncEconomicIndexesAsync(ct);
        await SyncStockPricesAsync(ct);
        _logger.LogInformation("Market data synchronization completed successfully.");
    }

    public async Task SyncPtaxRatesAsync(DateTime? targetDate = null, CancellationToken ct = default)
    {
        var date = targetDate ?? DateTime.Today;
        var dateStr = date.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
        var url = $"https://olinda.bcb.gov.br/olinda/servico/PTAX/versao/v1/odata/CotacaoMoedaDia(moeda=@moeda,dataCotacao=@dataCotacao)?@moeda='USD'&@dataCotacao='{dateStr}'&$format=json";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<BcbPtaxODataResponse>(url, ct);
            var item = response?.Value?.FirstOrDefault();

            if (item != null)
            {
                var buyRate = item.CotacaoCompra;
                var sellRate = item.CotacaoVenda;

                var existingPtax = await _unitOfWork.Repository<PtaxRate>().FindAsync(p => p.RateDate.Date == date.Date, ct);
                var ptax = existingPtax.FirstOrDefault();

                if (ptax == null)
                {
                    ptax = new PtaxRate
                    {
                        RateDate = date.Date,
                        BuyRate = buyRate,
                        SellRate = sellRate
                    };
                    await _unitOfWork.Repository<PtaxRate>().AddAsync(ptax, ct);
                }
                else
                {
                    ptax.BuyRate = buyRate;
                    ptax.SellRate = sellRate;
                    _unitOfWork.Repository<PtaxRate>().Update(ptax);
                }

                // Also update general ExchangeRate (USD -> BRL)
                var existingFx = await _unitOfWork.Repository<ExchangeRate>().FindAsync(e => e.FromCurrency == Currency.USD && e.ToCurrency == Currency.BRL && e.RateDate.Date == date.Date, ct);
                var fx = existingFx.FirstOrDefault();
                if (fx == null)
                {
                    await _unitOfWork.Repository<ExchangeRate>().AddAsync(new ExchangeRate
                    {
                        FromCurrency = Currency.USD,
                        ToCurrency = Currency.BRL,
                        RateDate = date.Date,
                        Rate = sellRate
                    }, ct);
                }
                else
                {
                    fx.Rate = sellRate;
                    _unitOfWork.Repository<ExchangeRate>().Update(fx);
                }

                await _unitOfWork.SaveChangesAsync(ct);
                _logger.LogInformation("PTAX USD rates for {Date} synced successfully: Buy={Buy:N4}, Sell={Sell:N4}", date.ToShortDateString(), buyRate, sellRate);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync PTAX rates for date {Date}", dateStr);
        }
    }

    public async Task SyncStockPricesAsync(CancellationToken ct = default)
    {
        var investments = await _unitOfWork.Repository<Investment>().FindAsync(i => i.ValuationType == ValuationType.TickerMarket && !string.IsNullOrEmpty(i.Ticker), ct);
        var tickers = investments.Select(i => i.Ticker!).Distinct().ToList();

        if (!tickers.Any())
        {
            _logger.LogInformation("No active tickers found for market price sync.");
            return;
        }

        var symbolsStr = string.Join(",", tickers);
        var url = $"https://query1.finance.yahoo.com/v7/finance/quote?symbols={symbolsStr}";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<YahooQuoteResponseDto>(url, ct);
            var results = response?.QuoteResponse?.Result;

            if (results != null)
            {
                var today = DateTime.Today;
                foreach (var quote in results)
                {
                    if (string.IsNullOrEmpty(quote.Symbol)) continue;

                    var price = quote.RegularMarketPrice;
                    var currency = quote.Currency == "BRL" ? Currency.BRL : Currency.USD;

                    var existing = await _unitOfWork.Repository<MarketPrice>().FindAsync(m => m.Ticker == quote.Symbol && m.PriceDate.Date == today, ct);
                    var record = existing.FirstOrDefault();

                    if (record == null)
                    {
                        await _unitOfWork.Repository<MarketPrice>().AddAsync(new MarketPrice
                        {
                            Ticker = quote.Symbol,
                            PriceDate = today,
                            ClosingPrice = price,
                            Currency = currency
                        }, ct);
                    }
                    else
                    {
                        record.ClosingPrice = price;
                        _unitOfWork.Repository<MarketPrice>().Update(record);
                    }
                }

                await _unitOfWork.SaveChangesAsync(ct);
                _logger.LogInformation("Stock prices updated for {Count} tickers.", results.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync stock prices from Yahoo Finance.");
        }
    }

    public async Task SyncEconomicIndexesAsync(CancellationToken ct = default)
    {
        var today = DateTime.Today;

        // Sync CDI (Series 12)
        await SyncBcbSeriesIndexAsync(12, IndexBenchmark.CDI, today, ct);

        // Sync TR (Series 226 - FGTS)
        await SyncBcbSeriesIndexAsync(226, IndexBenchmark.TR, today, ct);
    }

    private async Task SyncBcbSeriesIndexAsync(int seriesCode, IndexBenchmark indexCode, DateTime today, CancellationToken ct)
    {
        var url = $"https://api.bcb.gov.br/dados/serie/bcdata.sgs.{seriesCode}/dados/ultimos/1?formato=json";
        try
        {
            var list = await _httpClient.GetFromJsonAsync<List<BcbSgsPointDto>>(url, ct);
            var item = list?.FirstOrDefault();
            if (item != null && decimal.TryParse(item.Valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var rateValue))
            {
                var dailyRate = rateValue / 100m; // Convert percentage to factor
                var existing = await _unitOfWork.Repository<EconomicIndex>().FindAsync(e => e.IndexCode == indexCode && e.IndexDate.Date == today, ct);
                var record = existing.FirstOrDefault();

                if (record == null)
                {
                    await _unitOfWork.Repository<EconomicIndex>().AddAsync(new EconomicIndex
                    {
                        IndexCode = indexCode,
                        IndexDate = today,
                        DailyRate = dailyRate
                    }, ct);
                }
                else
                {
                    record.DailyRate = dailyRate;
                    _unitOfWork.Repository<EconomicIndex>().Update(record);
                }

                await _unitOfWork.SaveChangesAsync(ct);
                _logger.LogInformation("Synced Economic Index {IndexCode}: {Rate:P4}", indexCode, dailyRate);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync BCB economic index series {SeriesCode}", seriesCode);
        }
    }
}

// DTO Helper Classes
internal record BcbPtaxODataResponse([property: JsonPropertyName("value")] List<BcbPtaxItemDto>? Value);
internal record BcbPtaxItemDto(
    [property: JsonPropertyName("cotacaoCompra")] decimal CotacaoCompra,
    [property: JsonPropertyName("cotacaoVenda")] decimal CotacaoVenda
);
internal record BcbSgsPointDto(
    [property: JsonPropertyName("data")] string Data,
    [property: JsonPropertyName("valor")] string Valor
);
internal record YahooQuoteResponseDto([property: JsonPropertyName("quoteResponse")] YahooQuoteResultDto? QuoteResponse);
internal record YahooQuoteResultDto([property: JsonPropertyName("result")] List<YahooQuoteItemDto>? Result);
internal record YahooQuoteItemDto(
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("regularMarketPrice")] decimal RegularMarketPrice,
    [property: JsonPropertyName("currency")] string Currency
);
