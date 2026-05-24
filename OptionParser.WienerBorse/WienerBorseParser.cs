using log4net;
using OptionParser.Core;
using OptionParser.Core.Domain.Interfaces;
using OptionParser.WienerBorse.Models;
using System;
using System.Collections.Generic;
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
            return [];
        }
    }
}
