using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace Api
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly IEventPublisher _eventPublisher;

        public GreeterService(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _eventPublisher.Publish(new Events.GreetingSubmittedEvent { Name = request.Name, SubmittedAt = DateTime.UtcNow });

            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
