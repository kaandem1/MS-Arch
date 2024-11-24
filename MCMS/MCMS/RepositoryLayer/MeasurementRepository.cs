using MCMS.Data;
using MCMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMS.RepositoryLayer
{
    public class MeasurementRepository : IMeasurementRepository
    {
        private readonly AppDbContext _dbContext;

        public MeasurementRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Measurement> FindByIdAsync(int id)
        {
            return await _dbContext.Measurements.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Measurement> InsertAsync(Measurement entity)
        {
            await _dbContext.Measurements.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(Measurement entity)
        {
            var existingDevice = await this.FindByIdAsync(entity.Id);
            _dbContext.Measurements.Attach(entity);

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
            var device = await _dbContext.Measurements.FirstOrDefaultAsync(d => d.Id == id);
            if (device != null)
            {
                _dbContext.Measurements.Remove(device);
                await _dbContext.SaveChangesAsync();
            }
        }




    }
}