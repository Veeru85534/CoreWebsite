using System.ComponentModel.DataAnnotations;

namespace CoreTechStatic.Models.Banner
{
    public class BannerPage
    {
        [Key]
        public int BannerId { get; set; }
        public string? BannerImage { get; set; }
        public int? Slide { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}
