using UserMS.Core.DomainLayer.Models;

namespace UserMS.Logic.ServiceLayer.IServices
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user);
    }
}
