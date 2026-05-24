using AngleSharp.Html.Parser;
using log4net;
using OptionParser.Core;
using OptionParser.Core.Domain.Interfaces;
using OptionParser.WienerBorse.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionParser.WienerBorse
{
    public class WienerBorseParser: BaseParser<ParsedItem>
    {
        public const string SiteUrl = "https://www.wienerborse.at/en/bonds/";
        public WienerBorseParser(ILog log, ICSVExporter<ParsedItem> exporter, HttpClient client) : base(log, exporter, client, "WienerBorse") { }

        public override async Task<IEnumerable<ParsedItem>> ParseAllPages()
        {
            string? currentPage = SiteUrl;
            var items = new List<ParsedItem>();
            int pageCounter = 1;
            var htmlParser = new HtmlParser();

            while (currentPage is not null) 
            {
                Console.WriteLine($"Парсинг страницы {pageCounter}");
                logger.Info($"Start parsing page {pageCounter}");
                var pageInfo = await this.client.GetAsync(currentPage);

                string content = await pageInfo.Content.ReadAsStringAsync();
                Console.WriteLine(content);

                using var document = await htmlParser.ParseDocumentAsync(content);

                var rows = document.QuerySelectorAll(".table-horizontal > tbody > tr");
                foreach(var row in rows)
                {
                    var cells = row.QuerySelectorAll("td");
                    if (cells.Length < 9) 
                        continue;

                    var item = new ParsedItem();

                    item.Name = cells[0].TextContent.Trim();
                    item.Last = cells[1].TextContent.Trim();

                    var changeSpans = cells[2].QuerySelectorAll("span");

                    string changePercent = changeSpans.Length > 0 ? changeSpans[0].TextContent.Trim() : "";
                    string changeAbsolute = changeSpans.Length > 1 ? changeSpans[1].TextContent.Trim() : "";

                    item.ChangePercent = changePercent;
                    item.ChangeAbsolute = changeAbsolute;

                    item.DateTime = cells[3].InnerHtml.Replace("<br>", " ").Trim();
                    item.ISIN = cells[4].TextContent.Trim();
                    item.BidVolume = cells[5].TextContent.Replace("\n", " ").Trim();
                    item.AskVolume = cells[6].TextContent.Replace("\n", " ").Trim();
                    item.Maturity = cells[7].TextContent.Trim();

                    var status = cells[8].QuerySelector("span");
                    item.Status = status?.TextContent?.Trim() ?? "";

                    items.Add(item);
                }
                logger.Info($"End parsing page {pageCounter}");

                pageCounter++;
                var nextButton = document.QuerySelector("li.next a");
                string? nextHref = nextButton?.GetAttribute("href");

                if (nextHref != null)
                {
                    currentPage = $"https://www.wienerborse.at{nextHref}";
                }
                else
                {
                    currentPage = null;
                }

                if (currentPage is null)
                    logger.Info("Finished parsing WienerBorse");
                else
                    await Task.Delay(Random.Shared.Next(300, 700));
            }
            return items;
        }
    }
}
