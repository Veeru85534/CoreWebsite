using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace CoreTechStatic.Models
{
    public class Cart
    {
        [Key]
        public int Cart_Id { get; set; }
        public int Product_Id { get; set; }
        public int SubItemId { get; set; }
        public int Quntity { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int OrderStatus { get; set; }
        public decimal? Gst { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? SubTotalPrice { get; set; }
        public int? PriceId { get; set; }
        public int? UserId { get; set; }        
        [ForeignKey("PriceId")]
        public virtual PriceMaster PriceMaster { get; set; }
       // public List<Products> Products { get; set; }    
        [ForeignKey("Product_Id")]
        public virtual Products Products { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
