using MCMS.Models.DTOModels;
using MCMS.RepositoryLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMS.RepositoryLayer
{
    public interface IDeviceInfoRepository : IRepository<DeviceInfoDTO>
    {
        Task<DeviceInfoDTO> FindByIdAsync(int id);
        Task<DeviceInfoDTO> InsertAsync(DeviceInfoDTO entity);
        Task UpdateAsync(DeviceInfoDTO entity);
        Task DeleteByIdAsync(int id);

    }
}