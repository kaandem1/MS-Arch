using MCMS.Models.DTOModels;

namespace MCMS.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        public long Timestamp { get; set; }
        public int DeviceId { get; set; }
        public float MeasurementValue { get; set; }

        public DeviceInfoDTO Device { get; set; }
    }
}
