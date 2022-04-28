using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace TestUserProject.Models
{
    public class RabbitMqCommand : IRabbitMqCommand
    {
        private readonly IConfiguration _configuration;

        public RabbitMqCommand(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMessage(string message)
        {
            var connectionstring = _configuration.GetConnectionString("RabbitMq");
            var factory = new ConnectionFactory() { Uri = new System.Uri(connectionstring), UserName ="admin", Password= "bPueSsZvU5hexP4J" };
            factory.Ssl.Enabled = true;
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "CommandQueue",
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                               routingKey: "CommandQueue",
                               basicProperties: null,
                               body: body);
            }
        }
    }
}
