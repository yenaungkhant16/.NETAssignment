﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SA4102CASoftware.Models
{
	public class PurchasesTable
	{
		public Guid OrderId { get; set; }
		public string ProductId { get; set; }
		public string ProductName { get; set; }
		public string ProductDescription { get; set; }
		public string ProductImage { get; set; }
		public DateTime OrderDate { get; set; }
		public int Quantity { get; set; }
		public double Rating { get; set; }
		public Guid ActivationCode { get; set; }
	}
}


