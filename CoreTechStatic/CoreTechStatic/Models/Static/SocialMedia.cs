using System.ComponentModel.DataAnnotations;

namespace CoreTechStatic.Models.Static
{
    public class SocialMedia
    {
        [Key]
        public int Id { get; set; }
        public string? SMName { get; set; }
        public string? Link { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
