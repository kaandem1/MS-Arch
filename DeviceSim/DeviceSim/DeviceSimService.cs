using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSim
{
    public class DeviceSimService : IHostedService
    {
        private readonly IDeviceSimulator _deviceSimulator;
        private readonly ILogger<DeviceSimService> _logger;
        private Timer _timer;

        public DeviceSimService(IDeviceSimulator deviceSimulator, ILogger<DeviceSimService> logger)
        {
            _deviceSimulator = deviceSimulator;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Device Simulator Service is starting.");

            await Task.Delay(30000, cancellationToken);

            _timer = new Timer(ExecuteDeviceSimulators, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }



        private async void ExecuteDeviceSimulators(object state)
        {
            try
            {
                await _deviceSimulator.SendSimulatedValuesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while simulating devices.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Device Simulator Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
