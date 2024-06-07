using Microsoft.AspNetCore.Connections;
using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using System.Threading.Channels;

namespace komikaan.Irrigator.Contexts
{
    public class GardenerContext
    {
        private IModel _channel;

        public Task StartAsync(CancellationToken token)
        {

            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();


            _channel.ExchangeDeclare("stop-notifications", "direct", true);
            _channel.QueueDeclare(queue: "gardeners",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _channel.QueueBind("gardeners", "stop-notifications", "gardener");

            return Task.CompletedTask;
        }

        public void SendMessage(object message)
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            var rawMessage = JsonSerializer.Serialize(message, options);
            var body = Encoding.UTF8.GetBytes(rawMessage);
            _channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "gardeners",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
