using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreTechStatic.Models
{
    public class Order
    {        
        [Key]
        public int OrderId { get; set; }
        public int OrderStatus { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public int NoOfItem { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? PackedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int  CartSubID { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string PinCode { get; set; }
        [ForeignKey("CustomerId")]
        public virtual User User { get; set; }
    }
}
