using EventsDispatcher.Interfaces;
using Microsoft.Extensions.Configuration;
using  Microsoft.Extensions.DependencyInjection;

namespace EventsDispatcher.IoC
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterEventsDispatcher(
            this IServiceCollection service, 
            IConfiguration configuration)
        {
            service.AddSingleton<IRabbitConfigurationProvider>(
                new RabbitConfigurationProvider(
                    new RabbitMQConfig(configuration)));
            service.AddSingleton<IRabbitConnection, RabbitConnection>();
            service.AddSingleton<IEventBusDispatcher, EventBusDispatcher>();

            return service;
        }
    }
}