using System;
using RabbitMQ.Client;

namespace EventsDispatcher.Interfaces
{
    public interface IRabbitConnection : IDisposable
    {
        IConnection GetConnection();
        IModel GetChannel();
        void Close();
    }
}