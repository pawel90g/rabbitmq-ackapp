using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsumerWorker.Services.Interfaces;
using EventsDispatcher.Interfaces;
using EventsSubscriber.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Shared.Events;

namespace ConsumerWorker
{
    public class QueueConsumerService : IHostedService
    {
        private readonly ILogger<QueueConsumerService> _logger;
        private readonly IEventBusSubscriber eventBusSubscriber;
        private readonly IEventBusDispatcher eventBusDispatcher;
        private readonly IConsumerWorkerConfigProvider consumerWorkerConfigProvider;

        public QueueConsumerService(
            ILogger<QueueConsumerService> logger,
            IEventBusSubscriber eventBusSubscriber,
            IConsumerWorkerConfigProvider consumerWorkerConfigProvider, IEventBusDispatcher eventBusDispatcher)
        {
            _logger = logger;
            this.eventBusSubscriber = eventBusSubscriber;
            this.consumerWorkerConfigProvider = consumerWorkerConfigProvider;
            this.eventBusDispatcher = eventBusDispatcher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            Console.WriteLine($"Start subscribing exchange: {consumerWorkerConfigProvider.GetExchangeName()}, routingKey: {consumerWorkerConfigProvider.GetRoutingKey()}, mode: {consumerWorkerConfigProvider.GetExchangeType()}");

            await eventBusSubscriber.SubscribeExchangeAsync<CustomEvent>(
                consumerWorkerConfigProvider.GetRoutingKey(),
                consumerWorkerConfigProvider.GetExchangeType(),
                async (message) => {
                    Console.WriteLine($"[Consumer {consumerWorkerConfigProvider.GetWorkerName()}] Message received: {message.Message}");
                    await eventBusDispatcher.ExchangePublishAsync("Message received", "ackExchange", "ack");
                });

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stop");
            return Task.CompletedTask;
        }
    }
}
