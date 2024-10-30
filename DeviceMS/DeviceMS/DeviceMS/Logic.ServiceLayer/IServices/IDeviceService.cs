using DeviceMS.Core.DomainLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceMS.Logic.ServiceLayer.IServices
{
    public interface IDeviceService
    {
        Task<Device> GetAsync(int id);
        Task<Device> CreateAsync(Device device);
        Task UpdateAsync(Device device);
        Task<List<Device>> GetAllAsync();
        Task<IEnumerable<Device>> SearchByNameAsync(string name);
        Task<IEnumerable<Device>> SearchByAddressAsync(string address);
        Task<Device> FindByIdAsync(int id);

        Task AssignDeviceToUserAsync(int deviceId, int userId);
        Task RemoveDeviceFromUserAsync(int deviceId);
        Task ClearDevicesForUserAsync(int userId);

        Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId);

        Task<List<Device>> GetUnownedDevicesAsync();

    }
}
