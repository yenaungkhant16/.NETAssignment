using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SA4102CASoftware.Models
{
	public class Products
	{
		public Products()
		{
			this.OrderDetails = new List<OrderDetails>();
		}

		[Key]
		[Required]
		public string ProductId { get; set; }

		[Required]
		public string ProductName { get; set; }

		[Required]
		[Range(0, double.MaxValue)]
		public double Price { get; set; }

		[Required]
		public string ProductDescription { get; set; }

		[Required]
		public string ProductImage { get; set; }

		public virtual ICollection<OrderDetails> OrderDetails { get; set; }
	}
}


