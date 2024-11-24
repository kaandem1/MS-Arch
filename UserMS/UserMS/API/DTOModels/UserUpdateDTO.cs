using UserMS.Core.DomainLayer.Models;

namespace UserMS.API.DTOModels
{
    public class UserUpdateDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
    }
}
