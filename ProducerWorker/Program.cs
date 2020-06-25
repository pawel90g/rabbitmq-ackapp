using EventsDispatcher.IoC;
using EventsDispatcher.Models.Enums;
using EventsSubscriber.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProducerWorker.Services;
using ProducerWorker.Services.Interfaces;
using System.Linq;

namespace ProducerWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var argsList = args.ToList();
                    var queueName = argsList.FirstOrDefault(a => a.StartsWith("queue"))?.Split("=").ElementAtOrDefault(1);

                    var exchangeName = argsList.FirstOrDefault(a => a.StartsWith("exchange"))?.Split("=").ElementAtOrDefault(1);
                    var exchangeType = ExchangeType.Parse(argsList.FirstOrDefault(a => a.StartsWith("type"))?.Split("=").ElementAtOrDefault(1));
                    var routingKey = argsList.FirstOrDefault(a => a.StartsWith("routingKey"))?.Split("=").ElementAtOrDefault(1);

                    var productionIntervalStr = argsList.FirstOrDefault(a => a.StartsWith("interval"))?.Split("=").ElementAtOrDefault(1);
                    var isValidInterval = int.TryParse(productionIntervalStr, out var productionInterval);
                    var workerName = argsList.FirstOrDefault(a => a.StartsWith("worker"))?.Split("=").ElementAtOrDefault(1) ?? "Noname";

                    var cfgProvider = string.IsNullOrEmpty(exchangeName) || string.IsNullOrEmpty(routingKey)
                        ? new ProducerWorkerConfigProvider(queueName, 
                            string.IsNullOrEmpty(productionIntervalStr) && isValidInterval
                            ? default 
                            : productionInterval,
                            workerName)
                        : new ProducerWorkerConfigProvider(exchangeName, 
                            exchangeType,
                            routingKey,
                            string.IsNullOrEmpty(productionIntervalStr) && isValidInterval
                            ? default 
                            : productionInterval,
                            workerName);

                    services.AddSingleton<IProducerWorkerConfigProvider>(cfgProvider);

                    services.RegisterEventsDispatcher(hostContext.Configuration);
                    services.RegisterEventsSubscriber(hostContext.Configuration);
                    services.AddHostedService<ProducerServiceWorker>();
                });
    }
}
