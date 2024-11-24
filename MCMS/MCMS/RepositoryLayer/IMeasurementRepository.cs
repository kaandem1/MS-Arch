using MCMS.RepositoryLayer;
using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Models;

namespace MCMS.RepositoryLayer
{
    public interface IMeasurementRepository : IRepository<Measurement>
    {
        Task<Measurement> FindByIdAsync(int id);
        Task<Measurement> InsertAsync(Measurement entity);
        Task UpdateAsync(Measurement entity);
        Task DeleteByIdAsync(int id);

    }
}