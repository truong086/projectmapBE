using projectmap.Common;

namespace projectmap.Models
{
    public class RepairRecord : BaseEntity
    {
        public int? RD_id { get; set; } 
        public RepairDetails? repairDetails { get; set; }

        public int? TE_id { get; set; }
        public TrafficEquipment? trafficEquipment { get; set; }

        public int? Engineer_id { get; set; }
        public User? user { get; set; }

        public DateTimeOffset? RecordTime { get; set; } = DateTimeOffset.UtcNow;
        public string? NotificationRecord { get; set; }
    }
}
