using DeviceMS.Core.DomainLayer.Models;

namespace DeviceMS.API.DTOModels
{
    public class DeviceDTO
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public float MaxHourlyCons { get; set; }
    }
}
