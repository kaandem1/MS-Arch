namespace MCMS.Models
{
    public class Measurement
    {
        public long Timestamp { get; set; }
        public int DeviceId { get; set; }
        public float MeasurementValue { get; set; }
    }
}
