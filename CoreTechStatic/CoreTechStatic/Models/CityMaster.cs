using System.ComponentModel.DataAnnotations;

namespace CoreTechStatic.Models
{
    public class CityMaster
    {
        [Key]
        public int CityId { get; set; }
        public string CityName { get; set; }    
        public int StateId { get; set; }

    }
}
