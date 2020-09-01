using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var endpoint = _configuration.GetConnectionString("rabbit", "queue").Split(':');

            var factory = new ConnectionFactory() { HostName = endpoint[0], Port = int.Parse(endpoint[1]) };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "worker-greetings", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind("worker-greetings", "greetings", "");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var @event = JsonSerializer.Deserialize<GreetingSubmittedEvent>(ea.Body.Span);
                        _logger.LogInformation($"Got greeting from {@event.Name}");
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetRequiredService<GreetingsDbContext>();
                            db.Set<Greeting>().Add(new Greeting { Name = @event.Name, SubmittedAt = @event.SubmittedAt });
                            db.SaveChanges();
                        }                            
                    };

                    channel.BasicConsume(queue: "worker-greetings", autoAck: true, consumer: consumer);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    }
                }
            }
        }
    }
}
