using UserMS.Core.DomainLayer.Models;

namespace UserMS.API.DTOModels
{
    public class UserChangePasswordDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
