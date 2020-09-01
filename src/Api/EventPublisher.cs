using Events;
using RabbitMQ.Client;
using System;
using System.Text.Json;

namespace Api
{
    public interface IEventPublisher
    {
        void Publish(GreetingSubmittedEvent @event);
    }

    public class EventPublisherBuilder
    {
        private string _host;
        private int _port;

        public EventPublisherBuilder WithEndpoint(string endpoint)
        {
            var splitEndpoint = endpoint.Split(':');
            _host = splitEndpoint[0];
            _port = int.Parse(splitEndpoint[1]);
            return this;
        }

        public IEventPublisher Build()
        {
            var factory = new ConnectionFactory() { HostName = _host, Port = _port };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("greetings", ExchangeType.Fanout);
            return new EventPublisher(connection, channel);
        }

        private class EventPublisher : IEventPublisher, IDisposable
        {
            private readonly IConnection _connection;
            private readonly IModel _model;

            public EventPublisher(IConnection connection, IModel model)
            {
                _connection = connection;
                _model = model;
            }

            public void Publish(GreetingSubmittedEvent @event)
            {
                _model.BasicPublish("greetings", "", true, null, JsonSerializer.SerializeToUtf8Bytes(@event).AsMemory());
            }

            public void Dispose()
            {
                _model?.Dispose();
                _connection?.Dispose();
            }
        }
    }

}
