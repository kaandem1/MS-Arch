using UserMS.Core.DomainLayer.Enums;
using UserMS.Core.DomainLayer.Models;

namespace UserMS.API.DTOModels
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public UserRole Role { get; set; }
    }
}
