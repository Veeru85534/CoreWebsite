using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreTechStatic.Models
{
    public class User
    {
        [Key]
        public int? User_Id { get; set; }
        public string User_Name { get; set; }
        public string First_Name { get; set; }  
        public string Last_Name { get; set;}
        public string? Role { get; set; }
        public string Phone_No { get; set; }
        public string? Email_Id { get; set; }
        public DateTime Created_Date { get; set; }
        public string Address { get; set; }
        public string? ShippingAddress { get; set; }
        public int? City { get; set; }
        public int? State { get; set; }
        public string? Country { get; set; }
        public string? Pin_Code { get; set; }
        public int? Otp { get; set; }
        public string Password { get; set; }
        [ForeignKey("State")]
        public virtual StateMaster StateMaster { get; set; }
        [ForeignKey("City")]
        public virtual CityMaster CityMaster { get; set; }


    }
}
