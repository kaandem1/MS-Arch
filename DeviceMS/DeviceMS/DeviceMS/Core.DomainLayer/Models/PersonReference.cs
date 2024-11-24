using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace DeviceMS.Core.DomainLayer.Models
{
    public class PersonReference
    {
        public int UserId { get; set; }
    }
}
