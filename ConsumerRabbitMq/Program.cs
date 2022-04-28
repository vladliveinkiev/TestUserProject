using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace ConsumerRabbitMq
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { Uri = new Uri("amqps://b-0ec05adf-fb49-456f-9e3e-e3d8efe342de.mq.eu-central-1.amazonaws.com:5671"),
                UserName ="admin", Password= "bPueSsZvU5hexP4J"};
            factory.Ssl.Enabled = true;
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "CommandQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, aruments) =>
            {
                var body = aruments.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: "CommandQueue",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
