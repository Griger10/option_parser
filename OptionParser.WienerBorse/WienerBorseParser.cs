using AngleSharp.Html.Parser;
using log4net;
using OptionParser.Core;
using OptionParser.Core.Domain.Interfaces;
using OptionParser.WienerBorse.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptionParser.WienerBorse
{
    public class WienerBorseParser : BaseParser<ParsedItem>
    {
        public const string SiteUrl = "https://www.wienerborse.at/en/bonds/";
        public WienerBorseParser(ILog log, ICSVExporter<ParsedItem> exporter, HttpClient client)
            : base(log, exporter, client, "WienerBorse") { }

        public override async Task<IEnumerable<ParsedItem>> ParseAllPages()
        {
            var firstResponse = await this.client.GetAsync(SiteUrl);
            string content = await firstResponse.Content.ReadAsStringAsync();

            var items = new ConcurrentBag<ParsedItem>();
            var htmlParser = new HtmlParser();
            using var document = await htmlParser.ParseDocumentAsync(content);
            var lastNode = document.QuerySelector("ul.pagination > li:nth-last-child(2)");
            int pagesCount = lastNode != null ? int.Parse(lastNode.TextContent.Trim()) : 1;

            Console.WriteLine($"Доступно страниц: {pagesCount}");

            var chunks = Enumerable.Range(1, pagesCount).Chunk(50);
            var options = new ParallelOptions { MaxDegreeOfParallelism = 5 };
            int parsedPagesCount = 0;

            await Parallel.ForEachAsync(chunks, options, async (chunk, token) =>
            {
                var htmlParser = new HtmlParser();

                foreach (var page in chunk)
                {
                    int currentCount = Interlocked.Increment(ref parsedPagesCount);
                    if (currentCount % 75 == 0 || currentCount == pagesCount)
                    {
                        double progress = (double)currentCount / pagesCount;
                        Console.WriteLine($"--- Спаршено {progress:P2} ({currentCount}/{pagesCount})");
                    }

                    logger.Info($"Start parsing page {page}");

                    string url = $"https://www.wienerborse.at/en/bonds/?c7928-page={page}&per-page=50";
                    var pageInfo = await this.client.GetAsync(url, token);
                    string content = await pageInfo.Content.ReadAsStringAsync(token);

                    using var document = await htmlParser.ParseDocumentAsync(content);
                    var rows = document.QuerySelectorAll(".table-horizontal > tbody > tr");

                    foreach (var row in rows)
                    {
                        var cells = row.QuerySelectorAll("td");
                        if (cells.Length < 9)
                            continue;

                        var item = new ParsedItem();

                        item.Name = cells[0].TextContent.Trim();
                        item.Last = cells[1].TextContent.Trim();

                        var changeSpans = cells[2].QuerySelectorAll("span");

                        item.ChangePercent = changeSpans.Length > 0 ? changeSpans[0].TextContent.Trim() : "";
                        item.ChangeAbsolute = changeSpans.Length > 1 ? changeSpans[1].TextContent.Trim() : "";

                        item.DateTime = cells[3].TextContent.Trim() == "-"
                            ? "-"
                            : cells[3].InnerHtml.Replace("<br>", " ").Trim();
                        item.ISIN = cells[4].TextContent.Trim();
                        item.BidVolume = cells[5].TextContent.Replace("\n", " ").Trim();
                        item.AskVolume = cells[6].TextContent.Replace("\n", " ").Trim();
                        item.Maturity = cells[7].TextContent.Trim();

                        var status = cells[8].QuerySelector("span");
                        item.Status = status?.TextContent?.Trim() ?? "";

                        items.Add(item);
                    }

                    await Task.Delay(Random.Shared.Next(50, 100), token);
                }
            });

            logger.Info("Finished parsing WienerBorse");

            return items;
        }
    }
}