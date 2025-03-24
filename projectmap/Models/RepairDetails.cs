using projectmap.Common;

namespace projectmap.Models
{
    public class RepairDetails : BaseEntity
    {
        public int? TE_id { get; set; }
        public TrafficEquipment? trafficEquipment { get; set; }
        public int? FaultCodes { get; set; }
        public int? RepairStatus{ get; set; }
        public int? MaintenanceEngineer { get; set; }
        public User? user { get; set; }
        public string? Remark{ get; set; }
        public DateTimeOffset ?FaultReportingTime{ get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? RepairCompletionTime { get; set; }
        public virtual ICollection<ImageRepair>? ImageRepairs { get; set; }

    }
}
