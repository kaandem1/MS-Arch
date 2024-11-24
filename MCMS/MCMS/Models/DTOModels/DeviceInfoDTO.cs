using MCMS.Models;

namespace MCMS.Models.DTOModels
{
    public class DeviceInfoDTO
    {
        public int Id { get; set; }
        public float MaxHourlyCons { get; set; }
        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}