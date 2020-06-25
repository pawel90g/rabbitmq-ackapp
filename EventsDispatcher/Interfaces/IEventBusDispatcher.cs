
using System.Threading.Tasks;
using EventsDispatcher.Models.Enums;
using Shared.Shared.Events;

namespace EventsDispatcher.Interfaces
{
    public interface IEventBusDispatcher
    {
        void QueuePublish(string message, string queue);

        void ExchangePublish(string message, string exchange, string routingKey, string exchangeType = ExchangeType.Direct);
        void ExchangePublish<TEvent>(TEvent @event, string exchangeType = ExchangeType.Direct)
            where TEvent : EventBase;

        Task QueuePublishAsync(string message, string queue);

        Task ExchangePublishAsync(string message, string exchange, string routingKey, string exchangeType = ExchangeType.Direct);
        Task ExchangePublishAsync<TEvent>(TEvent @event, string exchangeType = ExchangeType.Direct)
            where TEvent : EventBase;
    }
}