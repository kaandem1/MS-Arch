using MCMS.Models;
using MCMS.Models.DTOModels;
using MCMS.RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMS.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceConsumptionRepository _deviceConsumptionRepository;

        public DeviceService(IDeviceConsumptionRepository deviceConsumptionRepository)
        {
            _deviceConsumptionRepository = deviceConsumptionRepository;
        }

        public async Task CreateDeviceAsync(DeviceInfoDTO deviceInfo)
        {
            if (deviceInfo == null)
                throw new ArgumentNullException(nameof(deviceInfo));

            var deviceConsumption = new DeviceConsumption
            {
                DeviceId = deviceInfo.Id,
                MaxHourlyCons = deviceInfo.MaxHourlyCons,
                HourlyConsumption = new Dictionary<long, float>()
            };

            await _deviceConsumptionRepository.InsertAsync(deviceConsumption);
        }

        public async Task UpdateDeviceConsumptionAsync(int deviceId, long timestamp, float consumption)
        {
            var deviceConsumption = await _deviceConsumptionRepository.FindByIdAsync(deviceId);

            if (deviceConsumption != null)
            {
                deviceConsumption.HourlyConsumption[timestamp] = consumption;

                await _deviceConsumptionRepository.UpdateAsync(deviceConsumption);
            }
        }

        public async Task UpdateMaxHourlyConsAsync(int deviceId, float maxHourlyCons)
        {
            var deviceConsumption = await _deviceConsumptionRepository.FindByIdAsync(deviceId);

            if (deviceConsumption != null)
            {
                deviceConsumption.MaxHourlyCons = maxHourlyCons;

                await _deviceConsumptionRepository.UpdateAsync(deviceConsumption);
            }
        }

        public async Task DeleteDeviceAsync(int deviceId)
        {
            if (deviceId == 0)
            {
                throw new ArgumentException("Invalid device ID.");
            }

            await _deviceConsumptionRepository.DeleteByIdAsync(deviceId);
        }
    }
}
