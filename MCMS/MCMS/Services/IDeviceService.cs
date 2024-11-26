using MCMS.Models.DTOModels;
using System.Threading.Tasks;

namespace MCMS.Services
{
    public interface IDeviceService
    {
        Task CreateDeviceAsync(DeviceInfoDTO deviceInfo);
        Task UpdateDeviceConsumptionAsync(int deviceId, long timestamp, float consumption);
        Task UpdateMaxHourlyConsAsync(int deviceId, float maxHourlyCons);

        Task DeleteDeviceAsync(int deviceId);
    }
}
