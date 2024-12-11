using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MCMS.Models.DTOModels;
using MCMS.Models;
using System.Threading.Tasks;
using MCMS.Data;


namespace MCMS.Services
{
    public class RabbitMQConsumer
    {
        private readonly IDeviceService _deviceService;
        private readonly AppDbContext _dbContext;
        private readonly WebSocketService _webSocketService;
        private readonly string[] _hostnames = new[]
        {
            "host.docker.internal",
            "rabbitmq",
            "localhost",
            "172.17.0.1"
        };
        private readonly Dictionary<int, List<(float, long)>> _deviceReadings = new Dictionary<int, List<(float, long)>>();

        public RabbitMQConsumer(IDeviceService deviceService, AppDbContext dbContext, WebSocketService webSocketService)
        {
            _deviceService = deviceService;
            _dbContext = dbContext;
            _webSocketService = webSocketService;
        }

        private const int MaxRetries = 10;
        private const int RetryDelay = 5000;

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            int retryCount = 0;

            while (retryCount < MaxRetries && !cancellationToken.IsCancellationRequested)
            {
                foreach (var hostname in _hostnames)
                {
                    try
                    {
                        Console.WriteLine($"Attempting to connect to RabbitMQ at {hostname}...");
                        var factory = new ConnectionFactory
                        {
                            HostName = hostname,
                            UserName = "admin",
                            Password = "admin",
                            Port = 5672,
                            VirtualHost = "/"
                        };

                        var connection = factory.CreateConnection("MCMS-Consumer");
                        var channel = connection.CreateModel();

                        Console.WriteLine($"Connected to RabbitMQ at {hostname}");
                        await StartConsuming(channel, cancellationToken);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to connect to RabbitMQ at {hostname}: {ex.Message}");
                    }
                }

                retryCount++;
                if (retryCount < MaxRetries && !cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Retrying to connect to RabbitMQ after a delay...");
                    await Task.Delay(RetryDelay, cancellationToken);
                }
                else
                {
                    Console.WriteLine("Max retry attempts reached. RabbitMQ is unavailable.");
                    throw new Exception("Unable to connect to RabbitMQ after maximum retries.");
                }
            }
        }

        private async Task StartConsuming(IModel channel, CancellationToken cancellationToken)
        {
            string deviceQueueName = "device-queue";
            string deviceInfoQueueName = "DeviceInfoQueue";

            channel.QueueDeclare(queue: deviceQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: deviceInfoQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    channel.Close();
                    return;
                }

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    if (ea.RoutingKey == deviceQueueName)
                    {
                        var deviceMessage = JsonSerializer.Deserialize<DeviceMessage>(message);
                        if (deviceMessage != null)
                        {
                            await ProcessDeviceMessage(deviceMessage);
                        }
                    }
                    else if (ea.RoutingKey == deviceInfoQueueName)
                    {
                        var deviceInfo = JsonSerializer.Deserialize<DeviceInfoDTO>(message);
                        if (deviceInfo != null)
                        {
                            await ProcessDeviceInfo(deviceInfo);
                        }
                    }

                    channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    channel.BasicReject(ea.DeliveryTag, requeue: true);
                }
            };

            channel.BasicConsume(queue: deviceQueueName, autoAck: false, consumer: consumer);
            channel.BasicConsume(queue: deviceInfoQueueName, autoAck: false, consumer: consumer);

            Console.WriteLine("[x] Waiting for messages...");

            await Task.Delay(-1, cancellationToken);
        }


        private async Task StartConsuming(IModel channel)
        {
            string deviceQueueName = "device-queue";
            string deviceInfoQueueName = "DeviceInfoQueue";

            channel.QueueDeclare(queue: deviceQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: deviceInfoQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    if (ea.RoutingKey == deviceQueueName)
                    {
                        var deviceMessage = JsonSerializer.Deserialize<DeviceMessage>(message);
                        if (deviceMessage != null)
                        {
                            await ProcessDeviceMessage(deviceMessage);
                        }
                    }
                    else if (ea.RoutingKey == deviceInfoQueueName)
                    {
                        var deviceInfo = JsonSerializer.Deserialize<DeviceInfoDTO>(message);
                        if (deviceInfo != null)
                        {
                            await ProcessDeviceInfo(deviceInfo);
                        }
                    }

                    channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    channel.BasicReject(ea.DeliveryTag, requeue: true);
                }
            };

            channel.BasicConsume(queue: deviceQueueName, autoAck: false, consumer: consumer);
            channel.BasicConsume(queue: deviceInfoQueueName, autoAck: false, consumer: consumer);

            Console.WriteLine("[x] Waiting for messages...");
            await Task.Delay(-1);
        }

        private async Task ProcessDeviceInfo(DeviceInfoDTO deviceInfo)
        {
            switch (deviceInfo.Operation)
            {
                case "CREATE":
                    await _deviceService.CreateDeviceAsync(deviceInfo);
                    break;

                case "UPDATE":
                    Console.WriteLine($"Updating MaxHourlyCons for device ID {deviceInfo.Id} to {deviceInfo.MaxHourlyCons}");
                    await _deviceService.UpdateMaxHourlyConsAsync(deviceInfo.Id, deviceInfo.MaxHourlyCons);
                    break;

                case "DELETE":
                    Console.WriteLine($"Deleting device with ID {deviceInfo.Id}");
                    await _deviceService.DeleteDeviceAsync(deviceInfo.Id);
                    break;

                default:
                    Console.WriteLine($"Unknown operation type: {deviceInfo.Operation}");
                    break;
            }
            Console.WriteLine($"[x] Processed DeviceInfo message: {JsonSerializer.Serialize(deviceInfo)}");
        }

        private async Task ProcessDeviceMessage(DeviceMessage deviceMessage)
        {
            Console.WriteLine($"[x] Received Device Message:");
            Console.WriteLine($"    Timestamp: {deviceMessage.Timestamp}");
            Console.WriteLine($"    Device ID: {deviceMessage.DeviceId}");
            Console.WriteLine($"    Measurement Value: {deviceMessage.MeasurementValue}");

            await ProcessHourlyConsumption(deviceMessage);
        }

        private async Task ProcessHourlyConsumption(DeviceMessage deviceMessage)
        {
            var deviceId = int.Parse(deviceMessage.DeviceId);
            var deviceConsumption = await _deviceService.GetDeviceConsumptionAsync(deviceId);

            if (deviceConsumption == null)
            {
                Console.WriteLine($"Device with ID {deviceId} not found in database.");
                return;
            }

            if (!_deviceReadings.ContainsKey(deviceId))
            {
                _deviceReadings[deviceId] = new List<(float, long)>();
            }

            _deviceReadings[deviceId].Add((deviceMessage.MeasurementValue, deviceMessage.Timestamp));

            if (_deviceReadings[deviceId].Count == 6)
            {
                var totalConsumption = _deviceReadings[deviceId].Sum(x => x.Item1);
                var timestamp = _deviceReadings[deviceId].Last().Item2;

                Console.WriteLine($"Device {deviceId} Hourly Consumption: {totalConsumption}");

                deviceConsumption.HourlyConsumption[timestamp] = totalConsumption;

                if (totalConsumption > deviceConsumption.MaxHourlyCons)
                {
                    var warningMessage = $"WARNING: Device {deviceId} exceeded max hourly consumption!";
                    await _webSocketService.SendNotification(warningMessage);
                    Console.WriteLine(warningMessage);
                }

                await _deviceService.UpdateDeviceConsumptionAsync(deviceId, timestamp, totalConsumption);
                _deviceReadings[deviceId].Clear();
            }
        }
    }
}
