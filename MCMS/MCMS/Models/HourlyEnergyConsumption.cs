namespace MCMS.Models
{
    public class HourlyEnergyConsumption
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime Hour { get; set; }
        public float TotalConsumption { get; set; }
        public bool ExceededLimit { get; set; }
    }

}
