using MCMS.Data;
using MCMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MCMS.RepositoryLayer
{
    public class DeviceInfoRepository : IDeviceConsumptionRepository
    {
        private readonly AppDbContext _dbContext;

        public DeviceInfoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DeviceConsumption> FindByIdAsync(int id)
        {
            Console.WriteLine($"Querying database for DeviceConsumption with deviceId: {id}");
            var consumption = await _dbContext.DeviceConsumptions
                                              .FirstOrDefaultAsync(d => d.DeviceId == id);

            if (consumption == null)
            {
                Console.WriteLine($"No consumption data found for deviceId: {id}");
            }
            else
            {
                Console.WriteLine($"Found consumption data for deviceId: {id}");
            }

            return consumption;
        }

        public async Task<DeviceConsumption> InsertAsync(DeviceConsumption entity)
        {
            await _dbContext.DeviceConsumptions.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(DeviceConsumption entity)
        {
            _dbContext.DeviceConsumptions.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var device = await _dbContext.DeviceConsumptions
                                         .FirstOrDefaultAsync(d => d.DeviceId == id);
            if (device != null)
            {
                _dbContext.DeviceConsumptions.Remove(device);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DeviceConsumption>> GetByDeviceIdAsync(int deviceId)
        {
            return await _dbContext.DeviceConsumptions
                                   .Where(d => d.DeviceId == deviceId)
                                   .ToListAsync();
        }
    }
}
