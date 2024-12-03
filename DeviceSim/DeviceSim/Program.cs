using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;

namespace DeviceSim
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Waiting for 30 seconds before starting DeviceSim...");
            Task.Delay(30000).Wait();
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var rabbitMQConfig = new RabbitMQConfig
                    {
                        HostName = context.Configuration["RabbitMQ:HostName"],
                        UserName = context.Configuration["RabbitMQ:UserName"],
                        Password = context.Configuration["RabbitMQ:Password"],
                        Port = Convert.ToInt32(context.Configuration["RabbitMQ:Port"]),
                        VirtualHost = context.Configuration["RabbitMQ:VirtualHost"],
                        QueueName = context.Configuration["RabbitMQ:QueueName"]
                    };

                    var deviceConfigs = context.Configuration.GetSection("DeviceConfigs")
                        .Get<List<DeviceConfig>>();

                    services.AddSingleton(rabbitMQConfig);
                    services.AddSingleton(deviceConfigs);
                    services.AddSingleton<IDeviceSimulator, DeviceSimulator>();
                    services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();
                    services.AddHostedService<DeviceSimService>();
                });
    }

    public class RabbitMQConfig
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string QueueName { get; set; }
    }

}
