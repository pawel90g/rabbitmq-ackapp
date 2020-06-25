namespace ProducerWorker.Services.Interfaces {
    public interface IProducerWorkerConfigProvider {

        string GetQueueName();

        string GetExchangeName();
        string GetExchangeType();
        string GetRoutingKey();

        string GetWorkerName();
        int GetProductionInterval();
        bool UseExchange();
    }
}