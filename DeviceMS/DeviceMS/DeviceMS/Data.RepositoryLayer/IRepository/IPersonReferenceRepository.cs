using DeviceMS.Core.DomainLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceMS.Data.RepositoryLayer.IRepository
{
    public interface IPersonReferenceRepository : IRepository<PersonReference>
    {
        Task<List<PersonReference>> GetAllAsync();
        Task<PersonReference> GetByIdAsync(int id);


    }
}

