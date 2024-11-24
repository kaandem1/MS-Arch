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
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "admin",
                Password = "admin"
            };

            // Create a connection
            using var connection = factory.CreateConnection("DeviceMS-Producer");
            // Create a channel
            using var channel = connection.CreateModel();

            string queueName = "DeviceInfoQueue";

            // Ensure the queue exists in case the consumer hasn't declared it yet
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Serialize the deviceInfo object to JSON
            var message = JsonSerializer.Serialize(deviceInfo);
            var body = Encoding.UTF8.GetBytes(message);

            // Publish the message to the queue
            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body);

            Console.WriteLine($"[x] Sent device info: Id={deviceInfo.Id}, MaxHourlyCons={deviceInfo.MaxHourlyCons}");
        }
    }
}