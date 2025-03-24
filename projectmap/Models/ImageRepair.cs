using projectmap.Common;

namespace projectmap.Models
{
    public class ImageRepair : BaseEntity
    {
        public int? Repair_id { get; set; }
        public RepairDetails? repairDetails { get; set; }
        public string? image { get; set; }
        public string? publicId { get; set; }
    }
}
