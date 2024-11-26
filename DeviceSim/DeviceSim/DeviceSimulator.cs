using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceSim
{
    public interface IDeviceSimulator
    {
        Task SendSimulatedValuesAsync();
    }

    public class DeviceSimulator : IDeviceSimulator
    {
        private readonly IConfiguration _configuration;
        private readonly IRabbitMQProducer _rabbitMQProducer;
        private readonly ILogger<DeviceSimulator> _logger;
        private readonly List<DeviceConfig> _deviceConfigs;
        private readonly List<DeviceMeasurement> _csvData;

        private readonly Dictionary<string, float> _lastSentValues;

        public DeviceSimulator(IConfiguration configuration, IRabbitMQProducer rabbitMQProducer, ILogger<DeviceSimulator> logger)
        {
            _configuration = configuration;
            _rabbitMQProducer = rabbitMQProducer;
            _logger = logger;

            _deviceConfigs = _configuration.GetSection("DeviceConfigs").Get<List<DeviceConfig>>();
            _csvData = ReadCsvData("sensor.csv");

            _lastSentValues = new Dictionary<string, float>();
        }

        public async Task SendSimulatedValuesAsync()
        {
            if (!_deviceConfigs.Any() || !_csvData.Any())
            {
                _logger.LogError("Device configurations or CSV data is empty. Cannot start simulation.");
                return;
            }

            int index = 0;
            int measurementCount = _csvData.Count;

            while (true)
            {
                var measurement = _csvData[index];
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                await SendDeviceMessageAsync("11", measurement.Value, timestamp);
                await SendDeviceMessageAsync("12", measurement.Value, timestamp);

                index = (index + 1) % measurementCount;
                await Task.Delay(10000);
            }
        }

        private async Task SendDeviceMessageAsync(string deviceId, float measurementValue, long timestamp)
        {
            if (!_lastSentValues.ContainsKey(deviceId) || _lastSentValues[deviceId] != measurementValue)
            {
                var message = new DeviceMessage
                {
                    Timestamp = timestamp,
                    DeviceId = deviceId,
                    MeasurementValue = measurementValue
                };

                await _rabbitMQProducer.SendMessageAsync(message);
                _logger.LogInformation($"Device {deviceId}: Value = {measurementValue}");

                _lastSentValues[deviceId] = measurementValue;
            }
            else
            {
                _logger.LogInformation($"Device {deviceId}: No value change, skipping message.");
            }
        }

        private List<DeviceMeasurement> ReadCsvData(string filePath)
        {
            var measurements = new List<DeviceMeasurement>();

            try
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                if (!File.Exists(fullPath))
                {
                    _logger.LogError($"CSV file not found: {fullPath}");
                    return measurements;
                }

                using (var reader = new StreamReader(fullPath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                }))
                {
                    csv.Context.RegisterClassMap<DeviceMeasurementMap>();

                    measurements = csv.GetRecords<DeviceMeasurement>().ToList();

                    _logger.LogInformation($"Loaded {measurements.Count} measurement(s) from {filePath}.");

                    for (int i = 0; i < measurements.Count; i++)
                    {
                        _logger.LogInformation($"Measurement {i + 1}: Value = {measurements[i].Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading CSV file: {ex.Message}");
            }

            return measurements;
        }
    }

    public class DeviceMeasurement
    {
        public float Value { get; set; }
    }

    public class DeviceMeasurementMap : ClassMap<DeviceMeasurement>
    {
        public DeviceMeasurementMap()
        {
            Map(m => m.Value).Index(0);
        }
    }

}
