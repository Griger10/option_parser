using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
