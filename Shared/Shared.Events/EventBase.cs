using Newtonsoft.Json;

namespace Shared.Shared.Events
{
    public abstract class EventBase
    {
        [JsonIgnore]
        public string RoutingKey { get; protected set; }
        [JsonIgnore]
        public long? Expiration { get; protected set; }

        protected EventBase() { }

        protected EventBase(string routingKey)
        {
            RoutingKey = routingKey;
            Expiration = null;
        }

        protected EventBase(string routingKey, long expiration)
        {
            RoutingKey = routingKey;
            Expiration = expiration;
        }
    }
}