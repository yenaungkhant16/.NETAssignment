using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SA4102CASoftware.Models
{
	public class ActivationCodes
	{
		public ActivationCodes()
		{

			this.ActivationCode = Guid.NewGuid();
		}

		[Key]
		[Required]
		public Guid ActivationCode { get; set; }

		[Required]
		public Guid OrderId { get; set; }

		[Required]
		public string ProductId { get; set; }

		[ForeignKey("OrderId, ProductId")]
		public virtual OrderDetails OrderDetails { get; set; }
	}
}


