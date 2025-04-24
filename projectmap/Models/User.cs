using projectmap.Common;

namespace projectmap.Models
{
    public class User : BaseEntity
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public int? Identity { get; set; }
        public int? UserStatus{ get; set; }
        public virtual ICollection<RepairRecord>? RepairRecords { get; set; }
        public virtual ICollection<RepairDetails>? RepairDetails { get; set; }

    }
}
