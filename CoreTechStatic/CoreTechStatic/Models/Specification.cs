using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreTechStatic.Models
{
    public class Specification
    {
        [Key]
        public int S_Id { get; set; }       
        public string? Name { get; set; }       
        public string S_type1 { get; set; }
        public string S_type2 { get; set; }
        public string? S_type3 { get; set; }
        public string? S_type4 { get; set; }
        public string? S_type5 { get; set; }
        public string? S_type6 { get; set; }
        public string? S_type7 { get; set; }                
        public string? S_type8 { get; set; }                
        public string? S_type9 { get; set; }                
        public string? S_type10 { get; set; }                
    }
}
