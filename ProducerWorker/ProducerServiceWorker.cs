using System;
using System.Threading;
using System.Threading.Tasks;
using EventsDispatcher.Interfaces;
using EventsSubscriber.Interfaces;
using EventsSubscriber.Models.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProducerWorker.Services.Interfaces;
using Shared.Shared.Events;

namespace ProducerWorker
{
    public class ProducerServiceWorker : BackgroundService
    {
        private readonly ILogger<ProducerServiceWorker> _logger;
        private readonly IEventBusDispatcher eventBusDispatcher;
        private readonly IEventBusSubscriber eventBusSubscriber;
        private readonly IProducerWorkerConfigProvider producerWorkerConfigProvider;

        public ProducerServiceWorker(
            ILogger<ProducerServiceWorker> logger,
            IEventBusDispatcher eventBusDispatcher,
            IProducerWorkerConfigProvider producerWorkerConfigProvider, IEventBusSubscriber eventBusSubscriber)
        {
            _logger = logger;
            this.eventBusDispatcher = eventBusDispatcher;
            this.producerWorkerConfigProvider = producerWorkerConfigProvider;
            this.eventBusSubscriber = eventBusSubscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (producerWorkerConfigProvider.UseExchange())
                Console.WriteLine($"Publishing on exchange '{producerWorkerConfigProvider.GetExchangeName()}' using routing '{producerWorkerConfigProvider.GetRoutingKey()}'. Exchange type: '{producerWorkerConfigProvider.GetExchangeType()}'");
            else
                Console.WriteLine($"Publishing on queue '{producerWorkerConfigProvider.GetQueueName()}'");

            await eventBusSubscriber.SubscribeExchangeAsync("ackExchange", "ack", ExchangeType.Direct, (message) => Console.WriteLine(message));

            var i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                await eventBusDispatcher.ExchangePublishAsync<CustomEvent>(new CustomEvent(producerWorkerConfigProvider.GetRoutingKey(), $"Message{++i}"));
                await Task.Delay(producerWorkerConfigProvider.GetProductionInterval(), stoppingToken);
            }
        }
    }
}
