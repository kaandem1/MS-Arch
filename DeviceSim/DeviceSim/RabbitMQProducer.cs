using RabbitMQ.Client;
using System;

namespace DeviceSim
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly RabbitMQConfig _config;

        public RabbitMQProducer(RabbitMQConfig config)
        {
            _config = config;
        }

        public async Task SendMessageAsync(DeviceMessage message)
        {
            var factory = new ConnectionFactory
            {
                HostName = _config.HostName,
                UserName = _config.UserName,
                Password = _config.Password,
                Port = _config.Port,
                VirtualHost = _config.VirtualHost
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _config.QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));

                channel.BasicPublish(exchange: "",
                                     routingKey: _config.QueueName,
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine($" [x] Sent message for Device {message.DeviceId}: {message.MeasurementValue}");
            }
        }
    }
}
