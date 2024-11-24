using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MCMS.Models.DTOModels;
using System.Threading.Tasks;

namespace MCMS.Services
{
    public class RabbitMQConsumer
    {
        private readonly DeviceService _deviceService;

        public RabbitMQConsumer(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        public async Task StartListeningAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "admin",
                Password = "admin"
            };

            using var connection = factory.CreateConnection("MCMS-Consumer");
            using var channel = connection.CreateModel();

            string deviceQueueName = "DeviceInfoQueue";

            channel.QueueDeclare(
                queue: deviceQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var deviceConsumer = new EventingBasicConsumer(channel);

            deviceConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var deviceInfo = JsonSerializer.Deserialize<DeviceInfoDTO>(message);
                if (deviceInfo != null)
                {
                    await _deviceService.CreateDeviceAsync(deviceInfo);
                    Console.WriteLine($"[x] Device created: Id={deviceInfo.Id}, MaxHourlyCons={deviceInfo.MaxHourlyCons}");
                }
            };

            channel.BasicConsume(queue: deviceQueueName, autoAck: true, consumer: deviceConsumer);

            Console.WriteLine("[x] Waiting for device messages...");
            await Task.Delay(-1);
        }
    }
}
