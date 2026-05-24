using log4net;
using Microsoft.Extensions.DependencyInjection;
using OptionParser.Core.Domain.Interfaces;
using OptionParser.Core.Exporters;
using OptionParser.WienerBorse;
using OptionParser.WienerBorse.Models;
using System.Numerics;


var config = new FileInfo("logging.config");

log4net.Config.XmlConfigurator.Configure(config);

ILog logger = LogManager.GetLogger("WienerBorse");

logger.Info($"Start parsing at {DateTime.Now:dd.mm.YYYY}");

var services = new ServiceCollection();

services.AddHttpClient();

services.AddSingleton<ILog>(logger);

services.AddTransient<ICSVExporter<ParsedItem>, CSVExporter<ParsedItem>>();

services.AddTransient<WienerBorseParser>();

var serviceProvider = services.BuildServiceProvider();

var parser = serviceProvider.GetRequiredService<WienerBorseParser>();

Console.WriteLine($"Целевой сайт: {parser.site} [url: {WienerBorseParser.SiteUrl}]");


logger.Info($"End parsing at {DateTime.Now:dd.mm.YYYY}");
