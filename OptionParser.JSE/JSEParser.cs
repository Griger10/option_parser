using AngleSharp.Html.Parser;
using log4net;
using OptionParser.Core;
using OptionParser.Core.Domain.Interfaces;
using OptionParser.JSE.Models;
using Polly;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OptionParser.JSE
{
    public class JSEParser : BaseParser<ParsedItem>
    {
        public const string SiteUrl = "https://clientportal.jse.co.za/reports/delta-option-and-structured-option-trades";
        public const string ApiUrl = "https://clientportal.jse.co.za/_vti_bin/JSE/DerivativesService.svc/GetTradeOptions";
        public const int RetryCount = 3;
        public JSEParser(ILog logger, ICSVExporter<ParsedItem> exporter, HttpClient client) :
            base(logger, exporter, client, "JSE")
        { }

        public override async Task<IEnumerable<ParsedItem>> ParseAllPages()
        {
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(r => (int)r.StatusCode == 429 || (int)r.StatusCode >= 500)
                .WaitAndRetryAsync(
                    retryCount: RetryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        string errorDetail = outcome.Exception != null
                            ? outcome.Exception.Message
                            : $"HTTP {(int)outcome.Result.StatusCode}";
                        Console.WriteLine($"\nЧто-то пошло не так: {errorDetail}. Повторная попытка {retryAttempt}/{RetryCount} через {timespan.TotalSeconds} сек.");

                        logger.Warn($"Retry {retryAttempt} due to this: {errorDetail}");
                    });
            try
            {
                var httpContent = new StringContent("", Encoding.UTF8, "application/json");
                var response = await retryPolicy.ExecuteAsync(() => this.client.PostAsync(ApiUrl, httpContent));
                response.EnsureSuccessStatusCode();

                List<ParsedItem> result = [];

                using var stream = await response.Content.ReadAsStreamAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponse = await JsonSerializer.DeserializeAsync<RootDTO>(stream, options);

                if (apiResponse?.GetTradeOptionsResult == null || apiResponse?.GetTradeOptionsResult.Count == 0)
                {
                    Console.WriteLine("Записи на сайте JSE отсутствуют");
                    return result;
                }

                foreach (var dto in apiResponse?.GetTradeOptionsResult)
                {
                    ParsedItem item = new ParsedItem();

                    item.ShortName = dto.ShortName?.ToString() ?? "-";
                    item.TradeDate = dto.TradeDate?.ToString() ?? "-";
                    item.TradeType = dto.TradeType?.ToString() ?? "-";
                    item.FutureExpiry = dto.FutureExpiry?.ToString() ?? "-";
                    item.Strike = dto.Strike?.ToString() ?? "-";
                    item.CallPut = dto.CallPut?.ToString() ?? "-";
                    item.Quantity = dto.Quantity?.ToString() ?? "-";
                    item.Vol = dto.Vol?.ToString() ?? "-";
                    item.Premium = dto.Premium?.ToString() ?? "-";
                    item.FuturesPrice = dto.FuturesPrice?.ToString() ?? "-";

                    result.Add(item);
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Что-то окончательно пошло не так: ", ex.Message);
                logger.Error("Unhandled exception: ", ex);
                throw;
            }
        }
    }
}
