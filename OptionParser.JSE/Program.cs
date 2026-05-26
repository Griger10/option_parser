using log4net;
using Microsoft.Extensions.DependencyInjection;
using OptionParser.Core.Domain.Interfaces;
using OptionParser.Core.Exporters;
using OptionParser.JSE;
using OptionParser.JSE.Models;
using System.Numerics;
using System.Reflection;

string basePath = AppDomain.CurrentDomain.BaseDirectory;
string configPath = Path.Combine(basePath, "logging.config");

var start = DateTime.Now;

var config = new FileInfo(configPath);

log4net.Config.XmlConfigurator.Configure(config);

ILog logger = LogManager.GetLogger("JSE");

logger.Info($"Program launched at {DateTime.Now:dd.MM.yyyy HH:mm}");

var services = new ServiceCollection();

services.AddHttpClient();

services.AddSingleton<ILog>(logger);

services.AddTransient<ICSVExporter<ParsedItem>, CSVExporter<ParsedItem>>();

services.AddTransient<JSEParser>();

var serviceProvider = services.BuildServiceProvider();

var parser = serviceProvider.GetRequiredService<JSEParser>();

Console.WriteLine("---------------------------------------------------------");

Console.WriteLine("Логи сохраняются в директорию Logs в каталоге исполняемого файла.");

Console.WriteLine($"Целевой сайт: {parser.site} [url: {JSEParser.SiteUrl}]");

Console.Write("Введите директорию сохранения файлов (для сохранения в директорию исполняемого файла оставьте пустой): ");
var outputDir = Console.ReadLine();

if (string.IsNullOrWhiteSpace(outputDir))
{
    outputDir = "./";
}

Console.WriteLine("Начало парсинга...");

await parser.Run(outputDir);

var totalTime = DateTime.Now - start;

Console.WriteLine($"Затрачено времени: {totalTime:mm\\:ss}");
Console.WriteLine("Парсинг завершен.");
Console.WriteLine("---------------------------------------------------------");

logger.Info($"Program finished at {DateTime.Now:dd.MM.yyyy HH:mm}");
