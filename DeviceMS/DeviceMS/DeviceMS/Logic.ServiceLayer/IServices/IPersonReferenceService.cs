using DeviceMS.Core.DomainLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceMS.Logic.ServiceLayer.IServices
{
    public interface IPersonReferenceService
    {
        Task<PersonReference> GetAsync(int id);
        Task<PersonReference> CreateAsync(PersonReference person);
        Task<List<PersonReference>> GetAllAsync();
        Task DeleteAsync(PersonReference person);

    }
}
