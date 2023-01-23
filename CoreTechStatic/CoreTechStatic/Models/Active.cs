using System.ComponentModel.DataAnnotations;

namespace CoreTechStatic.Models
{
    public class Active
    {
        [Key]
        public int Ac_Id { get; set; }
        public string Ac_Name { get; set; }
    }
}
