using System.ComponentModel.DataAnnotations;

namespace CoreTechStatic.Models
{
    public class Category
    {
        [Key]
        public int Ca_ID { get; set; }
        public string? Ca_Name { get; set; }
        public string? Ca_Image { get; set; }
        public string? Ca_Description { get; set; }     
        public List<Products> Products { get; set; }
    }
}
