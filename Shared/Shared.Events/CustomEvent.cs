using System;
using Newtonsoft.Json;

namespace Shared.Shared.Events
{
    public class CustomEvent : EventBase
    {
        [JsonProperty("message")]
        public string Message { get; private set; }

        private CustomEvent() {

        }
        public CustomEvent(string routingKey, string message) : base(routingKey)
        {
            this.Message = message;
        }

        public CustomEvent(string routingKey, string message, long expiration) : base(routingKey, expiration)
        {
            this.Message = message;
        }

    }
}
