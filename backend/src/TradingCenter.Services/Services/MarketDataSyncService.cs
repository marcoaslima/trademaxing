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

                var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
                var existingPtax = await _unitOfWork.Repository<PtaxRate>().FindAsync(p => p.RateDate == utcDate, ct);
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
                var existingFx = await _unitOfWork.Repository<ExchangeRate>().FindAsync(e => e.FromCurrency == Currency.USD && e.ToCurrency == Currency.BRL && e.RateDate == utcDate, ct);
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
        var assets = await _unitOfWork.Repository<Asset>().FindAsync(a => a.ValuationType == ValuationType.TickerMarket && !string.IsNullOrEmpty(a.Ticker), ct);
        if (!assets.Any())
        {
            _logger.LogInformation("No active tickers found in Master Asset Catalog for price sync.");
            return;
        }

        var today = DateTime.Today;
        int syncedCount = 0;

        foreach (var asset in assets)
        {
            var rawTicker = asset.Ticker!.Trim();
            var cleanSymbol = NormalizeTicker(rawTicker);

            if (string.IsNullOrEmpty(cleanSymbol)) continue;

            if (asset.Ticker != cleanSymbol)
            {
                asset.Ticker = cleanSymbol;
                _unitOfWork.Repository<Asset>().Update(asset);
            }

            decimal price = 0;
            var currency = asset.Currency;

            // 1. Try Yahoo Finance Chart v8 API
            var chartUrl = $"https://query2.finance.yahoo.com/v8/finance/chart/{cleanSymbol}?interval=1d";
            try
            {
                var chartRes = await _httpClient.GetFromJsonAsync<YahooChartResponseDto>(chartUrl, ct);
                var meta = chartRes?.Chart?.Result?.FirstOrDefault()?.Meta;
                if (meta != null && meta.RegularMarketPrice > 0)
                {
                    price = meta.RegularMarketPrice;
                    currency = string.Equals(meta.Currency, "BRL", StringComparison.OrdinalIgnoreCase) ? Currency.BRL : Currency.USD;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Yahoo Finance chart endpoint failed for {Symbol}, trying quote batch...", cleanSymbol);
            }

            // 2. Fallback to Yahoo Quote v7 API
            if (price <= 0)
            {
                var quoteUrl = $"https://query1.finance.yahoo.com/v7/finance/quote?symbols={cleanSymbol}";
                try
                {
                    var quoteRes = await _httpClient.GetFromJsonAsync<YahooQuoteResponseDto>(quoteUrl, ct);
                    var item = quoteRes?.QuoteResponse?.Result?.FirstOrDefault();
                    if (item != null && item.RegularMarketPrice > 0)
                    {
                        price = item.RegularMarketPrice;
                        currency = string.Equals(item.Currency, "BRL", StringComparison.OrdinalIgnoreCase) ? Currency.BRL : Currency.USD;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch price for {Symbol} from Yahoo Finance.", cleanSymbol);
                }
            }

            if (price > 0)
            {
                await UpsertMarketPriceAsync(cleanSymbol, today, price, currency, ct);
                if (!string.Equals(rawTicker, cleanSymbol, StringComparison.OrdinalIgnoreCase))
                {
                    await UpsertMarketPriceAsync(rawTicker, today, price, currency, ct);
                }
                syncedCount++;
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Stock prices updated for {Count} tickers.", syncedCount);
    }

    private async Task UpsertMarketPriceAsync(string symbol, DateTime date, decimal price, Currency currency, CancellationToken ct)
    {
        var targetDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var existing = await _unitOfWork.Repository<MarketPrice>().FindAsync(m => m.Ticker == symbol && m.PriceDate == targetDate, ct);
        var record = existing.FirstOrDefault();

        if (record == null)
        {
            await _unitOfWork.Repository<MarketPrice>().AddAsync(new MarketPrice
            {
                Ticker = symbol,
                PriceDate = date.Date,
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

    public static string NormalizeTicker(string? rawTicker)
    {
        if (string.IsNullOrWhiteSpace(rawTicker)) return string.Empty;
        var ticker = rawTicker.Trim().ToUpperInvariant();
        if (ticker.Contains(':'))
        {
            var parts = ticker.Split(':', 2);
            ticker = parts[1].Trim();
            if ((parts[0] == "BVMF" || parts[0] == "B3") && !ticker.EndsWith(".SA"))
            {
                ticker += ".SA";
            }
        }
        return ticker;
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
                var targetDate = DateTime.SpecifyKind(today.Date, DateTimeKind.Utc);
                var existing = await _unitOfWork.Repository<EconomicIndex>().FindAsync(e => e.IndexCode == indexCode && e.IndexDate == targetDate, ct);
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

public record BcbPtaxODataResponse([property: JsonPropertyName("value")] List<BcbPtaxItemDto>? Value);
public record BcbPtaxItemDto(
    [property: JsonPropertyName("cotacaoCompra")] decimal CotacaoCompra,
    [property: JsonPropertyName("cotacaoVenda")] decimal CotacaoVenda
);
public record BcbSgsPointDto(
    [property: JsonPropertyName("data")] string Data,
    [property: JsonPropertyName("valor")] string Valor
);
public record YahooQuoteResponseDto([property: JsonPropertyName("quoteResponse")] YahooQuoteResultDto? QuoteResponse);
public record YahooQuoteResultDto([property: JsonPropertyName("result")] List<YahooQuoteItemDto>? Result);
public record YahooQuoteItemDto(
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("regularMarketPrice")] decimal RegularMarketPrice,
    [property: JsonPropertyName("currency")] string Currency
);

public record YahooChartResponseDto([property: JsonPropertyName("chart")] YahooChartResultDto? Chart);
public record YahooChartResultDto([property: JsonPropertyName("result")] List<YahooChartItemDto>? Result);
public record YahooChartItemDto([property: JsonPropertyName("meta")] YahooChartMetaDto? Meta);
public record YahooChartMetaDto(
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("regularMarketPrice")] decimal RegularMarketPrice,
    [property: JsonPropertyName("currency")] string Currency
);
