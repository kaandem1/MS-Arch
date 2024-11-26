using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSim
{
    public class DeviceMessage
    {
        public long Timestamp { get; set; }
        public string DeviceId { get; set; }
        public float MeasurementValue { get; set; }
    }
}

