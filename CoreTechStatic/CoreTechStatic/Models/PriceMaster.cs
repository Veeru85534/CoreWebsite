using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreTechStatic.Models
{
    public class PriceMaster
    {
        [Key]
        public int PriceId { get; set; }
        public int ProductId { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}
        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }

    }
}
