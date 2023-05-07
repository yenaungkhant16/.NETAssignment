using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SA4102CASoftware.Models
{
	public class OrderDetails
	{
		public OrderDetails()
		{
			this.ActivationCodes = new List<ActivationCodes>();
		}

		[Key]
		[Required]
		[Column(Order = 1)]
		[Display(Name = "Order ID")]
		public Guid OrderId { get; set; }

		[Key]
		[Required]
		[Column(Order = 2)]
		[Display(Name = "Product ID")]
		public string ProductId { get; set; }

		[Required]
		[Range(0, int.MaxValue)]
		public int Quantity { get; set; }

		[Range(0, 5)]
		public double Rating { get; set; }

		[ForeignKey("OrderId")]
		public virtual Orders Orders { get; set; }

		[ForeignKey("ProductId")]
		public virtual Products Products { get; set; }

		public virtual ICollection<ActivationCodes> ActivationCodes { get; set; }
	}
}

