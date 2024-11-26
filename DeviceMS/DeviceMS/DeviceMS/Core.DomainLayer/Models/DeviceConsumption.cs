using System.Collections.Generic;

namespace MCMS.Models
{
    public class DeviceConsumption
    {
        public int DeviceId { get; set; }
        public float MaxHourlyCons { get; set; }

        public Dictionary<long, float> HourlyConsumption { get; set; } = new Dictionary<long, float>();
    }
}
