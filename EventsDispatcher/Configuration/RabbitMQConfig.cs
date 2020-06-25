using Microsoft.Extensions.Configuration;

namespace EventsDispatcher
{
    public class RabbitMQConfig
    {
        public string HostName { get; }
        public string UserName { get; }
        public string Password { get; }
        public ExchangeConfig ExchangeConfig { get; }
        public RabbitMQConfig(IConfiguration configuration)
        {
            HostName = configuration["RabbitMQ:HostName"];
            UserName = configuration["RabbitMQ:UserName"];
            Password = configuration["RabbitMQ:Password"];
            ExchangeConfig = new ExchangeConfig(long.Parse(configuration["RabbitMQ:Exchange:DefaultMessageExpiration"]));
        }
    }
}