using MCMS.Models;
using MCMS.Models.DTOModels;
using MCMS.RepositoryLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMS.RepositoryLayer
{
    public interface IDeviceConsumptionRepository : IRepository<DeviceConsumption>
    {
        Task<DeviceConsumption> FindByIdAsync(int id);
        Task<DeviceConsumption> InsertAsync(DeviceConsumption entity);
        Task UpdateAsync(DeviceConsumption entity);
        Task DeleteByIdAsync(int id);

    }
}