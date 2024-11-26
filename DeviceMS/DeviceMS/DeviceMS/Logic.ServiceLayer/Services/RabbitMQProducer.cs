using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using DeviceMS.Core.DomainLayer.Models;
using DeviceMS.API.DTOModels;


namespace DeviceMS.Logic.ServiceLayer.Services
{
   
    public class RabbitMQProducer
    {
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;

        public RabbitMQProducer(string hostName, string userName, string password)
        {
            _hostName = hostName;
            _userName = userName;
            _password = password;
        }

        public void SendDeviceInfo(DeviceInfoDTO deviceInfo)
        {
            int retries = 0;
            const int maxRetries = 10;
            const int retryDelay = 5000;

            while (retries < maxRetries)
            {
                try
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = "host.docker.internal",
                        UserName = "admin",
                        Password = "admin",
                        Port = 5672,
                        VirtualHost = "/"
                    };

                    using var connection = factory.CreateConnection("DeviceMS-Producer");
                    using var channel = connection.CreateModel();

                    string queueName = "DeviceInfoQueue";
                    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var message = JsonSerializer.Serialize(deviceInfo);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

                    Console.WriteLine($"[x] Sent message: {message}");
                    return;
                }
                catch (Exception ex)
                {
                    retries++;
                    Console.WriteLine($"Failed to send message. Retry {retries}/{maxRetries}. Error: {ex.Message}");
                    if (retries == maxRetries) throw;
                    Task.Delay(retryDelay).Wait();
                }
            }
        }
    }
}