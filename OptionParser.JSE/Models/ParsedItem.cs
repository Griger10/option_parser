using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OptionParser.JSE.Models
{
    public record ParsedItem
    {
        public string ShortName { get; set; }
        public string TradeDate { get; set; }
        public string TradeType { get; set; }
        public string FutureExpiry { get; set; }
        public string Strike { get; set; }
        public string CallPut { get; set; }
        public string Quantity { get; set; }
        public string Vol { get; set; }
        public string Premium { get; set; }
        public string FuturesPrice { get; set; }
    }


    public class RootDTO
    {
        [JsonPropertyName("GetTradeOptionsResult")]
        public List<ParsedItemDTO> GetTradeOptionsResult { get; set; }
    }

    public class ParsedItemDTO
    {
        public object ShortName { get; set; }
        public object TradeDate { get; set; }
        public object TradeType { get; set; }
        public object FutureExpiry { get; set; }
        public object Strike { get; set; }
        public object CallPut { get; set; }
        public object Quantity { get; set; }
        public object Vol { get; set; }
        public object Premium { get; set; }
        public object FuturesPrice { get; set; }
    }
}
