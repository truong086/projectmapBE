using projectmap.Models;

namespace projectmap.ViewModel
{
    public class RepairDetailsDTO
    {
        public int? traff_id { get; set; }
        public int? FaultCodes { get; set; }
        public int? RepairStatus { get; set; }
        public int? user_id { get; set; }
        public string? Remark { get; set; }
        public List<IFormFile>? images { get; set; }
    }
}
