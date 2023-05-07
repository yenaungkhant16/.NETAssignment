using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace SA4102CASoftware.Models
{
	public class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions<MyDbContext> options)
		: base(options)
		{ }

		public DbSet<Products> Products { get; set; }
		public DbSet<Customers> Customers { get; set; }
		public DbSet<Orders> Orders { get; set; }
		public DbSet<OrderDetails> OrderDetails { get; set; }
		public DbSet<ActivationCodes> ActivationCodes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<OrderDetails>()
				.HasKey(od => new { od.OrderId, od.ProductId });

            // Populate Dummy Datas
            var customers = new List<Customers>
			{
				new Customers { Email = "john@example.com", FirstName = "John", LastName = "Doe", Password = "password123" },
				new Customers { Email = "jane@example.com", FirstName = "Jane", LastName = "Doe", Password = "letmein" },
				new Customers { Email = "smith@example.com", FirstName = "Bob", LastName = "Smith", Password = "secret" },
				new Customers { Email = "alice@example.com", FirstName = "Alice", LastName = "Smith", Password = "password" },
				new Customers { Email = "david@example.com", FirstName = "David", LastName = "Brown", Password = "abc123" },
				new Customers { Email = "tomcat@example.com", FirstName = "Tom", LastName = "Cat", Password = "abc123456" },
				new Customers { Email = "edison@example.com", FirstName = "Edison", LastName = "Lee", Password = "Edison123" }
			};

            modelBuilder.Entity<Customers>().HasData(customers);

            var products = new List<Products>
			{
                new Products { ProductId = "P0001", ProductName = "Microsoft Office 365 Personal", Price = 69.99,
                    ProductDescription = "A comprehensive productivity suite from Microsoft, including Word, Excel, PowerPoint, and more, to help you accomplish your tasks efficiently.", ProductImage = "office365.jpg" },
                new Products { ProductId = "P0002", ProductName = "Adobe Photoshop CC", Price = 9.99,
                    ProductDescription = "A powerful and industry-standard photo editing software from Adobe, packed with advanced features and tools to enhance your creativity and productivity.", ProductImage = "photoshopcc.jpg" },
                new Products { ProductId = "P0003", ProductName = "Norton AntiVirus Plus", Price = 29.99,
                    ProductDescription = "A reliable antivirus software from Norton, providing advanced protection against viruses, malware, and other online threats, to keep your devices safe and secure.", ProductImage = "norton.jpg" },
                new Products { ProductId = "P0004", ProductName = "Zoom Meetings", Price = 99.9,
                    ProductDescription = "A popular and easy-to-use video conferencing software from Zoom, allowing you to host and join virtual meetings with colleagues, clients, and friends, from anywhere in the world.", ProductImage = "zoom.jpg" },
                new Products { ProductId = "P0005", ProductName = "AutoCAD LT", Price = 299.0,
                    ProductDescription = "A professional 2D CAD software from Autodesk, designed for creating, editing, and documenting your engineering and architectural designs, with precision and speed.", ProductImage = "autocadlt.jpg" },
                new Products { ProductId = "P0006", ProductName = "Final Cut Pro", Price = 299.99,
                    ProductDescription = "A high-performance and feature-rich video editing software from Apple, offering advanced tools and effects for creating stunning movies, TV shows, and videos, with unmatched speed.", ProductImage = "finalcutpro.jpg" },
                new Products { ProductId = "P0007", ProductName = "McAfee Total Protection", Price = 39.99,
                    ProductDescription = "A complete security software from McAfee, providing all-round protection against viruses, spyware, and other threats, for all your devices, to keep your digital life safe and secure.", ProductImage = "mcafee.jpg" },
                new Products { ProductId = "P0008", ProductName = "Visual Studio", Price = 299.0,
                    ProductDescription = "A powerful and flexible integrated development environment (IDE) from Microsoft, equipped with a wide range of tools and features, to help you create amazing apps and software.", ProductImage = "visualstudio.jpg" },
                new Products { ProductId = "P0009", ProductName = "Adobe Illustrator", Price = 19.99,
                    ProductDescription = "A versatile and industry-standard vector graphics editor from Adobe, allowing you to create stunning logos, icons, illustrations, and graphics, with precision and ease, for print, web, and mobile.", ProductImage = "illustrator.jpg" }

             };

            modelBuilder.Entity<Products>().HasData(products);

			var orders = new List<Orders>
			{
				new Orders { Email="john@example.com", OrderDate= new DateTime (2023, 04, 16, 20, 40, 30, 10) },
				new Orders { Email="john@example.com", OrderDate= new DateTime (2023, 03, 15, 10, 22, 20, 10) },
				new Orders { Email="smith@example.com", OrderDate= new DateTime (2023, 04, 10, 15, 15, 15, 15) },
				new Orders { Email="smith@example.com", OrderDate= new DateTime (2023, 02, 12, 20, 42, 30, 10) },
				new Orders { Email="jane@example.com", OrderDate= new DateTime (2023, 04, 09, 20, 10, 10, 10) },
				new Orders { Email="alice@example.com", OrderDate= new DateTime (2023, 04, 08, 20, 40, 30, 10) },
				new Orders { Email="david@example.com", OrderDate= new DateTime (2023, 04, 05, 10, 10, 30, 10) },
				new Orders { Email="david@example.com", OrderDate= new DateTime (2023, 04, 04, 23, 59, 30, 10) },
				new Orders { Email="john@example.com", OrderDate= new DateTime (2023, 03, 15, 10, 10, 10, 10) },
			};

			modelBuilder.Entity<Orders>().HasData(orders);

			var orderDetails = new List<OrderDetails>
			{
				new OrderDetails { OrderId = orders[0].OrderId, ProductId = products[0].ProductId, Quantity = 3, Rating=4 },
				new OrderDetails { OrderId = orders[0].OrderId, ProductId = products[1].ProductId, Quantity = 1, Rating=3 },
				new OrderDetails { OrderId = orders[0].OrderId, ProductId = products[4].ProductId, Quantity = 1, Rating=4 },
				new OrderDetails { OrderId = orders[1].OrderId, ProductId = products[1].ProductId, Quantity = 2, Rating=1 },
				new OrderDetails { OrderId = orders[1].OrderId, ProductId = products[0].ProductId, Quantity = 2, Rating=4 },
				new OrderDetails { OrderId = orders[2].OrderId, ProductId = products[2].ProductId, Quantity = 3, Rating=3 },
				new OrderDetails { OrderId = orders[3].OrderId, ProductId = products[3].ProductId, Quantity = 1, Rating=5 },
				new OrderDetails { OrderId = orders[4].OrderId, ProductId = products[4].ProductId, Quantity = 1, Rating=0 },
				new OrderDetails { OrderId = orders[5].OrderId, ProductId = products[0].ProductId, Quantity = 1, Rating=1 },
				new OrderDetails { OrderId = orders[6].OrderId, ProductId = products[1].ProductId, Quantity = 2, Rating=2 },
				new OrderDetails { OrderId = orders[7].OrderId, ProductId = products[2].ProductId, Quantity = 1, Rating=4 },
				new OrderDetails { OrderId = orders[8].OrderId, ProductId = products[3].ProductId, Quantity = 3, Rating=5 },
			};

			modelBuilder.Entity<OrderDetails>().HasData(orderDetails);

			var activationCodes = new List<ActivationCodes>();

			foreach (OrderDetails order in orderDetails)
			{
				for (int i = 0; i < order.Quantity; i++)
				{
					var temp = new ActivationCodes();
					temp.OrderId = order.OrderId;
					temp.ProductId = order.ProductId;
					activationCodes.Add(temp);
				}
			}

			modelBuilder.Entity<ActivationCodes>().HasData(activationCodes);
		}
	}
}

