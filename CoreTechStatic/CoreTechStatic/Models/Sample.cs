namespace CoreTechStatic.Models
{
    public class UserData 
    {
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalItem { get; set; }
        public int SubItemId { get; set; }  
        public string Address { get; set; }  
        public string PhoneNumber { get; set; }  
        public string PinCode { get; set; }  
    }
    public class SampleData
    {
        public int Cart_Id { get; set; }        
        public int NoOfItem { get; set; }
        public decimal Price { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Gst { get; set; }
    }
    public class Sample : UserData
    {
        public List<SampleData> SampleData { get; set; }
    }
}
