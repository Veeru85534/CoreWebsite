using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreTechStatic.Models
{
    public class WishList
    {
        [Key]
        public int WishListId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set;}
        public DateTime AddedDate { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }
    }
}
