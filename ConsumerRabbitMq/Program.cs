using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;
using System.Collections.Generic;

namespace ConsumerRabbitMq
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();

        static void Main()
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
                try
                {
                    var content = new StringContent("\""+message+"\"",Encoding.UTF8,"application/json");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NTM3MTkzMjZ9.mJgT2p5ccKyLGWmKMw17dafYX3a3wP5VNsAuKGz3PKo");
                    client.PostAsync(new Uri("https://localhost:5001/user/add"),content).
                    ContinueWith( task =>
                    {
                        string retv = "";
                        task.Result.Content.ReadAsStringAsync().ContinueWith(data=>
                        { retv = data.Result; });
                        if (task.IsCompletedSuccessfully)
                        {
                            Console.WriteLine($"User {message} added, GUID={retv}");
                        }
                        else
                        {
                            Console.WriteLine($"User {message} was not added");
                        }
                    }
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: "CommandQueue",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
        public class UserPost
        {
            public string User { get; set; }
        }
    }
}
