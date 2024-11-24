using MCMS.Data;
using MCMS.Models.DTOModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MCMS.RepositoryLayer
{
    public class DeviceInfoRepository : IDeviceInfoRepository
    {
        private readonly AppDbContext _dbContext;

        public DeviceInfoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DeviceInfoDTO> FindByIdAsync(int id)
        {
            return await _dbContext.Devices.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DeviceInfoDTO> InsertAsync(DeviceInfoDTO entity)
        {
            await _dbContext.Devices.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(DeviceInfoDTO entity)
        {
            _dbContext.Devices.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var device = await _dbContext.Devices.FirstOrDefaultAsync(d => d.Id == id);
            if (device != null)
            {
                _dbContext.Devices.Remove(device);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DeviceInfoDTO>> GetByDeviceIdAsync(int deviceId)
        {
            return await _dbContext.Devices
                                   .Where(d => d.Id == deviceId)
                                   .ToListAsync();
        }

    }
}
