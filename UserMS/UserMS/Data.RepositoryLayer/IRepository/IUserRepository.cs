using UserMS.Core.DomainLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace UserMS.Data.RepositoryLayer.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> FindByCredentials(string email, string password);
        Task<User> FindByEmailAsync(string email);
        Task<List<User>> GetAllAsync();

        Task<IEnumerable<User>> SearchByNameAsync(string name);

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        Task RollbackTransactionAsync(IDbContextTransaction transaction);
    }
}
