using System;
using System.Text;
using System.Threading.Tasks;
using EventsDispatcher.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Shared.Shared.Events;

namespace EventsDispatcher
{
    public class EventBusDispatcher : IEventBusDispatcher
    {
        private readonly IRabbitConfigurationProvider rabbitConfigurationProvider;
        private readonly IRabbitConnection rabbitConnection;
        private readonly ILogger<EventBusDispatcher> logger;

        public EventBusDispatcher(
            IRabbitConfigurationProvider rabbitConfigurationProvider,
            IRabbitConnection rabbitConnection,
            ILogger<EventBusDispatcher> logger
            )
        {
            this.logger = logger;
            this.rabbitConfigurationProvider = rabbitConfigurationProvider;
            this.rabbitConnection = rabbitConnection;
        }

        public void QueuePublish(string message, string queue)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            using (var channel = rabbitConnection.GetChannel())
            {
                channel.QueueDeclare(queue: queue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queue,
                                     basicProperties: null,
                                     body: body);
            }
        }

        public void ExchangePublish(string message, string exchange, string routingKey, string exchangeType = ExchangeType.Direct)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(exchange))
                throw new ArgumentNullException(nameof(exchange));
            if (string.IsNullOrEmpty(routingKey))
                throw new ArgumentNullException(nameof(routingKey));

            var exchangeCfg = rabbitConfigurationProvider.GetExchangeConfig();

            var channel = rabbitConnection.GetChannel();

            channel.ExchangeDeclare(exchange, exchangeType, durable: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            var expiration = exchangeCfg?.DefaultMessageExpiration;
            var basicProperties = expiration is null ? null : channel.CreateBasicProperties();
            if (basicProperties != null)
                basicProperties.Expiration = expiration.Value.ToString();

            channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: basicProperties,
                                 mandatory: false,
                                 body: body);
        }

        public void ExchangePublish<TEvent>(TEvent @event, string exchangeType = ExchangeType.Direct)
            where TEvent : EventBase
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var exchangeCfg = rabbitConfigurationProvider.GetExchangeConfig();

            var exchange = GetExchangeNameFromEvent<TEvent>();
            var channel = rabbitConnection.GetChannel();

            channel.ExchangeDeclare(exchange, exchangeType, durable: false, autoDelete: false, arguments: null);

            var body = SerializeEvent(@event);

            var expiration = @event.Expiration ?? exchangeCfg?.DefaultMessageExpiration;
            var basicProperties = expiration is null ? null : channel.CreateBasicProperties();
            if (basicProperties != null)
                basicProperties.Expiration = expiration.Value.ToString();

            channel.BasicPublish(exchange: exchange,
                                 routingKey: @event.RoutingKey,
                                 basicProperties: basicProperties,
                                 mandatory: false,
                                 body: body);
        }

        public Task QueuePublishAsync(string message, string queue)
        {
            return Task.Run(() =>
            {
                try
                {
                    QueuePublish(message, queue);
                }
                catch (Exception ex)
                {
                    logger.LogError("Error occures during event publishing", ex);
                }
            });
        }

        public Task ExchangePublishAsync(string message, string exchange, string routingKey, string exchangeType = ExchangeType.Direct)
        {
            return Task.Run(() =>
            {
                try
                {
                    ExchangePublish(message, exchange, routingKey, exchangeType);
                }
                catch (Exception ex)
                {
                    logger.LogError("Error occures during event publishing", ex);
                }
            });
        }

        public Task ExchangePublishAsync<TEvent>(TEvent @event, string exchangeType = ExchangeType.Direct)
            where TEvent : EventBase
        {
            return Task.Run(() =>
            {
                try
                {
                    ExchangePublish<TEvent>(@event, exchangeType);
                }
                catch (Exception ex)
                {
                    logger.LogError("Error occures during event publishing", ex);
                }
            });
        }

        private byte[] SerializeEvent(object @object) =>
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@object));

        private TEvent DeserializeEvent<TEvent>(byte[] bytes)
             where TEvent : EventBase =>
            JsonConvert.DeserializeObject<TEvent>(Encoding.UTF8.GetString(bytes));

        private static string GetExchangeName(EventBase @event) => @event.GetType().FullName;
        private static string GetExchangeNameFromEvent<TEvent>() where TEvent : EventBase => typeof(TEvent).FullName;
    }
}