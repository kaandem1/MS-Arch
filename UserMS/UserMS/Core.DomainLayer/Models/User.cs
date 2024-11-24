using UserMS.Core.DomainLayer.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace UserMS.Core.DomainLayer.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Country { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }

        public User Clone()
        {
            return new User
            {
                Id = this.Id,
                Country = this.Country,
                Email = this.Email,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Password = this.Password,
                Role = this.Role,
                Status = this.Status
            };
        }

    }
}
