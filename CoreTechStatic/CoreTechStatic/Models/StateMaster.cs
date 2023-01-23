using System.ComponentModel.DataAnnotations;

namespace CoreTechStatic.Models
{
    public class StateMaster
    {
        [Key]
        public int StateId { get; set; }
        public string StateName { get; set; }
    }
}
