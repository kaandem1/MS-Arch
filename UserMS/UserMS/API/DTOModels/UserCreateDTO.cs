using UserMS.Core.DomainLayer.Models;

namespace UserMS.API.DTOModels
{
    public class UserCreateDTO
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Country { get; set; }

        public string Password { get; set; }

    }
}
