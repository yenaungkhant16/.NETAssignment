using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SA4102CASoftware.Models
{
	public class Customers
	{
		public Customers()
		{
			this.Orders = new List<Orders>(); ;
		}
		[Key]
		[Display(Name = "Email Address")]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[StringLength(50)]
		public string Password { get; set; }

		[Required]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		public virtual ICollection<Orders> Orders { get; set; }
	}
}

