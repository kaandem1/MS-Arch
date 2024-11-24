using MCMS.Models.DTOModels;
using MCMS.RepositoryLayer;
using System.Threading.Tasks;

namespace MCMS.Services
{
    public class DeviceService
    {
        private readonly DeviceInfoRepository _repository;

        public DeviceService(DeviceInfoRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeviceInfoDTO> CreateDeviceAsync(DeviceInfoDTO device)
        {

            return await _repository.InsertAsync(device);
        }
    }
}
