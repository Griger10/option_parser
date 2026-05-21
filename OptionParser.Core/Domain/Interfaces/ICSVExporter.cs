using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionParser.Core.Domain.Interfaces
{
    public interface ICSVExporter<T>
    {
        public void ExportToCSV(IEnumerable<T> parsedItems, string filename);
    }
}
