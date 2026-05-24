using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionParser.WienerBorse.Models
{
    public record ParsedItem
    {
        public string Name { get; set; }
        public string Last { get; set; }

        public string ChangePercent { get; set; }
        public string ChangeAbsolute { get; set; }
        public string DateTime { get; set; }
        public string ISIN { get; set; }
        public string BidVolume { get; set; }
        public string AskVolume { get; set; }
        public string Maturity { get; set; }
        public string Status { get; set; }
    }
}
