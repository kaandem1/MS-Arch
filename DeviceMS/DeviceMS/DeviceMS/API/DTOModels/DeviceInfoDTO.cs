using DeviceMS.Core.DomainLayer.Models;

namespace DeviceMS.API.DTOModels
{
    public class DeviceInfoDTO
    {
        public int Id { get; set; }
        public float MaxHourlyCons { get; set; }
        public string Operation { get; set; } 

    }
}
