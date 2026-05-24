using log4net;
using Microsoft.Extensions.DependencyInjection;
using OptionParser.Core.Domain.Interfaces;
using OptionParser.Core.Exporters;
using OptionParser.WienerBorse;
using OptionParser.WienerBorse.Models;
using System.Numerics;
using System.Reflection;

log4net.Util.LogLog.InternalDebugging = true;

string basePath = AppDomain.CurrentDomain.BaseDirectory;
string configPath = Path.Combine(basePath, "logging.config");


var config = new FileInfo(configPath);

log4net.Config.XmlConfigurator.Configure( config);

ILog logger = LogManager.GetLogger("WienerBorse");

logger.Info($"Start parsing at {DateTime.Now:dd.MM.yyyy}");

var services = new ServiceCollection();

services.AddHttpClient();

services.AddSingleton<ILog>(logger);

services.AddTransient<ICSVExporter<ParsedItem>, CSVExporter<ParsedItem>>();

services.AddTransient<WienerBorseParser>();

var serviceProvider = services.BuildServiceProvider();

var parser = serviceProvider.GetRequiredService<WienerBorseParser>();

Console.WriteLine($"Целевой сайт: {parser.site} [url: {WienerBorseParser.SiteUrl}]");

Console.Write("Введите директорию для сохранения файлов (например, ./Output для сохранения в директорию исполняемого файла оставьте пустой): ");
var outputDir = Console.ReadLine();

if (string.IsNullOrWhiteSpace(outputDir))
{
    outputDir = "./";
}

Console.WriteLine("Начало парсинга...");

await parser.Run(outputDir);

Console.WriteLine("Парсинг завершился.");

logger.Info($"End parsing at {DateTime.Now:dd.MM.yyyy}");
