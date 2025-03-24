using System.ComponentModel.DataAnnotations;

namespace projectmap.Common
{
    public class BaseEntity
    {
        protected BaseEntity() { }
        [Key]
        public int id { get; set; }
        public bool deleted { get; set; }
        public string? cretoredit { get; set; }
    }
}
