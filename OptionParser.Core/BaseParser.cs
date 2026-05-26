using log4net;
using log4net.Util;
using OptionParser.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionParser.Core
{
    public abstract class BaseParser<T>
    {
        protected ILog logger;
        protected ICSVExporter<T> exporter;
        protected HttpClient client;
        public string site;

        public BaseParser(ILog log, ICSVExporter<T> exporter, HttpClient client, string site)
        {
            this.logger = log;
            this.exporter = exporter;
            this.client = client;
            this.site = site;
        }

        public async Task Run(string outputDirectory)
        {
            try
            {
                logger.Info("Start parsing into " + outputDirectory);
                var records = await ParseAllPages();
                string filename = $"{this.site}_{DateTime.Now:dd_MM_yyyy_HH_mm}.csv";
                string fullPath = Path.Combine(outputDirectory, filename);
                exporter.ExportToCSV(records, fullPath);
            }
            catch (Exception ex)
            {
                logger.Error("Unexpected behavior", ex);
                throw;
            }
            finally
            {
                logger.Info("Parsing finished");
            }
        }
        public abstract Task<IEnumerable<T>> ParseAllPages();
    }
}
