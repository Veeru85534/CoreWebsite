using System.ComponentModel.DataAnnotations;

namespace CoreTechStatic.Models
{
    public class HsnMaster
    {
        [Key]        
        public int HsnCode { get; set; }
        public string Description { get; set; }
        public decimal GST { get; set; }
        public decimal? SGSTorUGST { get; set; }
        public decimal? IGST { get; set; }
    }
}
