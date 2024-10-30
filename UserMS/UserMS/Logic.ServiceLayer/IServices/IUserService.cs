using UserMS.Core.DomainLayer.Models;

namespace UserMS.Logic.ServiceLayer.IServices
{
    public interface IUserService
    {
        Task<User> GetAsync(int id);

        Task<User> CreateAsync(User user);

        Task UpdateAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<User> FindByCredentialsAsync(string email, string password);
        Task<User> FindByEmailAsync(string email);
        Task<User> ChangePasswordAsync(User user, string newPassword);
        Task ResetPasswordAsync(string email, string newPassword);
        Task<List<User>> GetAllUserAsync();
        Task DeleteAsync(int id);
        Task<IEnumerable<User>> SearchByNameAsync(string name);
    }
}
