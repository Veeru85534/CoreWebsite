using System;
using System.ComponentModel.DataAnnotations;

namespace CoreTechStatic.Models.Static
{
	public class AboutUs
	{
		[Key]
		public int ID { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public string? Image { get; set; }
		public DateTime? LastUpdate { get; set; }
	}
}

