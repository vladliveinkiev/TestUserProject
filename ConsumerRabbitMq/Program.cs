using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;

namespace ConsumerRabbitMq
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();

        static void Main()
        {
            //create connection to RabbitMq
            try
            {
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri("amqps://b-0ec05adf-fb49-456f-9e3e-e3d8efe342de.mq.eu-central-1.amazonaws.com:5671"),
                    UserName = "admin",
                    Password = "bPueSsZvU5hexP4J"
                };
                factory.Ssl.Enabled = true;
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "CommandQueue",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                //Evend handler for consumer
                consumer.Received += (model, aruments) =>
                {
                    var body = aruments.Body.ToArray();
                    //Received message from the queue
                    var message = Encoding.UTF8.GetString(body);
                    try
                    {
                        // body content for post request
                        var content = new StringContent("\"" + message + "\"", Encoding.UTF8, "application/json");
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NTM3MTkzMjZ9.mJgT2p5ccKyLGWmKMw17dafYX3a3wP5VNsAuKGz3PKo");
                        client.PostAsync(new Uri("https://localhost:5001/user/add"), content).
                        ContinueWith(task =>
                        {
                            string retv = "";
                            task.Result.Content.ReadAsStringAsync().ContinueWith(data =>
                            {
                                retv = data.Result; // reading data from the response
                        });
                            if (task.IsCompletedSuccessfully && task.Result.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"User {message} added, GUID={retv}");
                            }
                            else
                            {
                                Console.WriteLine($"User {message} was not added, response {task.Result.StatusCode}, {task.Result.ReasonPhrase}");
                            }
                        }
                        );
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                };
                channel.BasicConsume(queue: "CommandQueue",
                                     autoAck: true,
                                     consumer: consumer);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
