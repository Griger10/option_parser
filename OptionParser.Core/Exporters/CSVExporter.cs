using OptionParser.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionParser.Core.Exporters
{
    public class CSVExporter<T> : ICSVExporter<T>
    {
        public void ExportToCSV(IEnumerable<T> parsedItems, string filename) 
        {
            Console.WriteLine("Stub");
        }
    }
}
