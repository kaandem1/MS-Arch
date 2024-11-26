using System.Collections.Generic;

namespace MCMS.Models
{
    public class DeviceMessage
    {
        public long Timestamp { get; set; }
        public string DeviceId { get; set; }
        public float MeasurementValue { get; set; }
    }
}
