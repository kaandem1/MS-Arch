using DeviceMS.Core.DomainLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceMS.Data.RepositoryLayer.IRepository
{
    public interface IDeviceRepository : IRepository<Device>
    {
        Task<List<Device>> GetAllAsync();
        Task<Device> GetByIdAsync(int id);
        Task<IEnumerable<Device>> SearchByNameAsync(string name);
        Task<IEnumerable<Device>> SearchByAddressAsync(string address);
        Task AssignDeviceToUserAsync(int deviceId, int userId);
        Task RemoveDeviceFromUserAsync(int deviceId);
        Task ClearDevicesForUserAsync(int userId);
        Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId);
        Task<List<Device>> GetUnownedDevicesAsync();

    }
}
