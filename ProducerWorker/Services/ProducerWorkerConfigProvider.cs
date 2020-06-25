using EventsDispatcher.Models.Enums;
using ProducerWorker.Services.Interfaces;

namespace ProducerWorker.Services
{
    public class ProducerWorkerConfigProvider : IProducerWorkerConfigProvider
    {
        private readonly string queueName;

        private readonly string exchangeName;
        private readonly string exchangeType;
        private readonly string routingKey;

        private readonly int productionInterval;
        private readonly string workerName;

        public ProducerWorkerConfigProvider(
            string queueName = "queue",
            int productionInterval = 1000,
            string workerName = null)
        {
            this.queueName = queueName;
            this.productionInterval = productionInterval;
            this.workerName = workerName;
        }

        public ProducerWorkerConfigProvider(
            string exchangeName = "exchange",
            string exchangeType = ExchangeType.Direct,
            string routingKey = "route",
            int productionInterval = 1000,
            string workerName = null)
        {
            this.exchangeName = exchangeName;
            this.exchangeType = exchangeType;
            this.routingKey = routingKey;
            this.productionInterval = productionInterval;
            this.workerName = workerName;
        }

        public string GetQueueName() => queueName;

        public string GetExchangeName() => exchangeName;
        public string GetExchangeType() => exchangeType;
        public string GetRoutingKey() => routingKey;

        public int GetProductionInterval() => productionInterval;
        public string GetWorkerName() => workerName;

        public bool UseExchange() => string.IsNullOrEmpty(queueName)
            && !string.IsNullOrEmpty(exchangeName);
    }
}