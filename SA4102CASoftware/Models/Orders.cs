using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SA4102CASoftware.Models
{
	public class Orders
	{
		public Orders()
		{
			this.OrderId = Guid.NewGuid();
			this.OrderDetails = new List<OrderDetails>();
		}

		[Key]
		[Required]
		public Guid OrderId { get; set; }

		[Display(Name = "Email Address")]
		[EmailAddress]
		[Required]
		public string Email { get; set; }

		[Required]
		public DateTime OrderDate { get; set; }

		[ForeignKey("Email")]
		public virtual Customers Customer { get; set; }

		public virtual ICollection<OrderDetails> OrderDetails { get; set; }
	}
}


