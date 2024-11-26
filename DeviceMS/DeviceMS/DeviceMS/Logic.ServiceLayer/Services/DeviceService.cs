using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceMS.Core.DomainLayer.Models;
using DeviceMS.Data.RepositoryLayer.IRepository;
using DeviceMS.Logic.ServiceLayer.IServices;
using DeviceMS.API.DTOModels;
using DeviceMS.Logic.ServiceLayer.Services;

namespace DeviceMS.Logic.ServiceLayer.Services
{
    public class DeviceService : IDeviceService
    {
        IDeviceRepository _deviceRepository;
        IPersonReferenceRepository _personReferenceRepository;
        private readonly RabbitMQProducer _rabbitMQProducer;

        public DeviceService(IDeviceRepository deviceRepository, IPersonReferenceRepository personReferenceRepository, RabbitMQProducer rabbitMQProducer)
        {
            _deviceRepository = deviceRepository;
            _personReferenceRepository = personReferenceRepository ?? throw new ArgumentNullException(nameof(personReferenceRepository));
            _rabbitMQProducer = rabbitMQProducer;
        }

        public async Task<Device> GetAsync(int id)
        {
            return await _deviceRepository.GetByIdAsync(id);
        }

        public async Task<Device> FindByIdAsync(int id)
        {
            return await _deviceRepository.FindByIdAsync(id);
        }

        public async Task<List<Device>> GetAllAsync()
        {
            return await _deviceRepository.GetAllAsync();
        }

        public async Task<Device> CreateAsync(Device device)
        {
            var createdDevice = await _deviceRepository.InsertAsync(device);

            return createdDevice;
        }

        public async Task UpdateAsync(Device device)
        {
            await _deviceRepository.UpdateAsync(device);
        }

        public async Task<IEnumerable<Device>> SearchByNameAsync(string name)
        {
            return await _deviceRepository.SearchByNameAsync(name);
        }

        public async Task<IEnumerable<Device>> SearchByAddressAsync(string address)
        {
            return await _deviceRepository.SearchByAddressAsync(address);
        }

        public async Task AssignDeviceToUserAsync(int deviceId, int userId)
        {
            var device = await _deviceRepository.GetByIdAsync(deviceId);
            if (device == null)
            {
                throw new KeyNotFoundException("Device not found");
            }

            var personReference = await _personReferenceRepository.GetByIdAsync(userId);
            if (personReference == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            device.User = personReference;

            await _deviceRepository.UpdateAsync(device);
        }
        public async Task RemoveDeviceFromUserAsync(int deviceId)
        {
            await _deviceRepository.RemoveDeviceFromUserAsync(deviceId);
        }

        public async Task ClearDevicesForUserAsync(int userId)
        {
            await _deviceRepository.ClearDevicesForUserAsync(userId);
        }

        public async Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId)
        {
            return await _deviceRepository.GetDevicesByUserIdAsync(userId);
        }

        public async Task<List<Device>> GetUnownedDevicesAsync()
        {
            return await _deviceRepository.GetUnownedDevicesAsync();
        }

        public async Task DeleteByIdAsync(int deviceId)
        {
            await _deviceRepository.DeleteByIdAsync(deviceId);
        }
    }
}
