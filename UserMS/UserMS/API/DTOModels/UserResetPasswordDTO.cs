using UserMS.Core.DomainLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace UserMS.API.DTOModels
{
    public class UserResetPasswordDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
