using DeviceMS.Core.DomainLayer.Data;
using DeviceMS.Core.DomainLayer.Models;
using DeviceMS.Data.RepositoryLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceMS.Data.RepositoryLayer.Repository
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly AppDbContext _dbContext;

        public DeviceRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Device>> GetAllAsync()
        {
            return await _dbContext.Devices.ToListAsync();
        }

        public async Task<Device> GetByIdAsync(int id)
        {
            return await _dbContext.Devices.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Device> InsertAsync(Device entity)
        {
            await _dbContext.Devices.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(Device entity)
        {
            var existingDevice = await this.GetByIdAsync(entity.Id);
            _dbContext.Devices.Attach(entity);

            _dbContext.Entry(entity).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {

                throw;
            }
        }

        public async Task DeleteByIdAsync(int id)
        {
            var device = await _dbContext.Devices.FirstOrDefaultAsync(d => d.Id == id);
            if (device != null)
            {
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Device>> SearchByNameAsync(string name)
        {
            return await _dbContext.Devices
                                 .Where(d => d.DeviceName.Contains(name))
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Device>> SearchByAddressAsync(string address)
        {
            return await _dbContext.Devices
                                 .Where(d => d.Address.Contains(address))
                                 .ToListAsync();
        }

        public async Task<Device> FindByIdAsync(int id)
        {
            return await _dbContext.Devices.FirstOrDefaultAsync(d => d.Id == id);
        }


        public async Task AssignDeviceToUserAsync(int deviceId, int userId)
        {
            var device = await _dbContext.Devices.FirstOrDefaultAsync(d => d.Id == deviceId);
            if (device == null)
            {
                throw new KeyNotFoundException($"Device with ID {deviceId} not found.");
            }
            device.User = new PersonReference { UserId = userId };
            _dbContext.Entry(device).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveDeviceFromUserAsync(int deviceId)
        {
            var device = await _dbContext.Devices.FirstOrDefaultAsync(d => d.Id == deviceId);

            if (device == null)
            {
                throw new KeyNotFoundException($"Device with ID {deviceId} not found.");
            }

            var userIdProperty = _dbContext.Entry(device).Property("UserId");
            userIdProperty.CurrentValue = null;

            device.User = null;

            _dbContext.Entry(device).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            var updatedDevice = await _dbContext.Devices.FindAsync(deviceId);

        }

        public async Task ClearDevicesForUserAsync(int userId)
        {
            var devices = await _dbContext.Devices.Where(d => d.User.UserId == userId).ToListAsync();

            foreach (var device in devices)
            {
                var userIdProperty = _dbContext.Entry(device).Property("UserId");
                userIdProperty.CurrentValue = null;
                device.User = null;
                _dbContext.Entry(device).State = EntityState.Modified;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Device>> GetDevicesByUserIdAsync(int userId)
        {
            return await _dbContext.Devices
                .Where(d => d.User != null && d.User.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Device>> GetUnownedDevicesAsync()
        {
            return await _dbContext.Devices
                .Where(device => device.User.UserId == null)
                .ToListAsync();
        }



    }
}
