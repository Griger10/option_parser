using OptionParser.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OptionParser.Core.Exporters
{
    public class CSVExporter<T> : ICSVExporter<T>
    {
        public void ExportToCSV(IEnumerable<T> parsedItems, string filename)
        {
            using var writer = new StreamWriter(filename);

            PropertyInfo[] properties = typeof(T).GetProperties();

            string headerLine = string.Join("\t", properties.Select(p => p.Name).ToArray());

            writer.WriteLine(headerLine);

            foreach (var record in parsedItems)
            {
                var recordValues = properties.Select(p =>
                {
                    var value = p.GetValue(record);
                    return value?.ToString()?.Trim() ?? "";
                });

                string line = string.Join("\t", recordValues);
                writer.WriteLine(line);
            }
        }
    }
}
