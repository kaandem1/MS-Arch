using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace DeviceSim
{
    public interface IDeviceSimulator
    {
        void StartSimulation();
        Task SendSimulatedValuesAsync();
    }

    public class DeviceSimulator : IDeviceSimulator
    {
        private readonly IConfiguration _configuration;
        private readonly IRabbitMQProducer _rabbitMQProducer;
        private readonly ILogger<DeviceSimulator> _logger;
        private readonly string _csvFilePath;
        private readonly List<string> _deviceIds;
        private readonly Dictionary<string, System.Timers.Timer> _timers;
        private readonly Dictionary<string, StreamReader> _csvReaders;

        public DeviceSimulator(IConfiguration configuration, IRabbitMQProducer rabbitMQProducer, ILogger<DeviceSimulator> logger)
        {
            _configuration = configuration;
            _rabbitMQProducer = rabbitMQProducer;
            _logger = logger;

            _csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "sensor.csv");
            _deviceIds = _configuration.GetSection("DeviceConfigs").Get<List<DeviceConfig>>().ConvertAll(d => d.DeviceId);
            _timers = new Dictionary<string, System.Timers.Timer>();
            _csvReaders = new Dictionary<string, StreamReader>();

            InitializeCsvReaders();
        }

        public async Task SendSimulatedValuesAsync()
        {
            foreach (var deviceId in _deviceIds)
            {
                await SimulateDeviceMeasurement(deviceId);
            }
        }

        public void StartSimulation()
        {
            if (_deviceIds.Count == 0)
            {
                _logger.LogError("No device configurations found. Simulation aborted.");
                return;
            }

            foreach (var deviceId in _deviceIds)
            {
                StartDeviceTimer(deviceId);
            }
        }

        private void InitializeCsvReaders()
        {
            foreach (var deviceId in _deviceIds)
            {
                try
                {
                    _csvReaders[deviceId] = new StreamReader(new FileStream(_csvFilePath, FileMode.Open, FileAccess.Read));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error initializing CSV reader for device {deviceId}: {ex.Message}");
                }
            }
        }

        private void StartDeviceTimer(string deviceId)
        {
            var timer = new System.Timers.Timer(10000);
            timer.Elapsed += async (sender, e) => await SimulateDeviceMeasurement(deviceId);
            timer.AutoReset = true;
            timer.Enabled = true;
            _timers[deviceId] = timer;
        }

        private async Task SimulateDeviceMeasurement(string deviceId)
        {
            try
            {
                if (_csvReaders[deviceId].EndOfStream)
                {
                    _logger.LogInformation($"End of CSV file reached for device {deviceId}. Resetting reader.");
                    _csvReaders[deviceId].BaseStream.Seek(0, SeekOrigin.Begin);
                    _csvReaders[deviceId].DiscardBufferedData();
                }

                var line = _csvReaders[deviceId].ReadLine();
                if (float.TryParse(line, out var measurementValue))
                {
                    var message = new DeviceMessage
                    {
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        DeviceId = deviceId,
                        MeasurementValue = measurementValue
                    };

                    await _rabbitMQProducer.SendMessageAsync(message);
                    _logger.LogInformation($"Sent message from Device {deviceId}: {JsonSerializer.Serialize(message)}");
                }
                else
                {
                    _logger.LogWarning($"Invalid data in CSV for device {deviceId}: {line}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error simulating measurement for device {deviceId}: {ex.Message}");
            }
        }
    }

    public class DeviceConfig
    {
        public string DeviceId { get; set; }
    }
}
