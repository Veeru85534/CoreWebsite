using System.ComponentModel.DataAnnotations;

namespace CoreTechStatic.Models
{
    public class Avalbility
    {
        [Key]
        public int Av_Id { get; set; }
        public string Av_Name { get; set; } 
    }
}
