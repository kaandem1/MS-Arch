using DeviceMS.Core.DomainLayer.Data;
using DeviceMS.Core.DomainLayer.Models;
using DeviceMS.Data.RepositoryLayer.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceMS.Data.RepositoryLayer.Repository
{
    public class PersonReferenceRepository : IPersonReferenceRepository
    {
        private readonly AppDbContext _dbContext;

        public PersonReferenceRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PersonReference>> GetAllAsync()
        {
            return await _dbContext.PersonReferences.ToListAsync();
        }

        public async Task<PersonReference> GetByIdAsync(int id)
        {
            return await _dbContext.PersonReferences.FirstOrDefaultAsync(p => p.UserId == id);
        }

        public async Task<PersonReference> InsertAsync(PersonReference entity)
        {
            await _dbContext.PersonReferences.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var person = await _dbContext.PersonReferences.FirstOrDefaultAsync(p => p.UserId == id);
            if (person != null)
            {
                _dbContext.PersonReferences.Remove(person);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(PersonReference entity)
        {
            throw new NotImplementedException();
        }

        public async Task<PersonReference> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }


    }
}
