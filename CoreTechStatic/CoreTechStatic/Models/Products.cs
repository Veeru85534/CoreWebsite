using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreTechStatic.Models
{
    public class Products
    {
        [Key]
        public int P_Id { get; set; }
        public string P_Name { get; set; }
        public string P_Image { get; set; }
        public string? P_ImageSec { get; set; }        
        public int Ca_Id { get; set; }
        public int S_Id { get; set; }
        public int? HsnCode { get; set; }
        public DateTime P_AddDate { get; set; }        
        public int? PriceID { get; set; }
        public string P_Description { get; set; }
        public string? Message { get; set; }
        public string? KeyWords { get; set; }
        public int Avalbility_Id { get; set; }
        public int Active_Id { get; set; }
        [ForeignKey("HsnCode")]
        public virtual HsnMaster HsnMaster { get; set; }
        [ForeignKey("PriceID")]
        public virtual PriceMaster PriceMaster { get; set; }
        [ForeignKey("S_Id")]
        public virtual Specification Specification { get; set; }
        [ForeignKey("Ca_Id")]
        public virtual Category Category { get; set; }
        [ForeignKey("Active_Id")]
        public virtual Active Active { get; set; }
        [ForeignKey("Avalbility_Id")]
        public virtual Avalbility Avalbility { get; set; }

    }
}
