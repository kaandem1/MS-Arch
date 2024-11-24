using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace DeviceMS.Core.DomainLayer.Models
{
    public class Device
    {
        [Key]
        public int Id { get; set; }
        public string DeviceName { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public float MaxHourlyCons { get; set; }

        public PersonReference? User { get; set; }
        
        public Device Clone()
        {
            return new Device
            {
                Id = this.Id,
                DeviceName = this.DeviceName,
                Description = this.Description,
                Address = this.Address,
                MaxHourlyCons = this.MaxHourlyCons
            };
        }

    }
}
