using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data
{
	public static class SeedData
	{
		public static void Initialize(IServiceProvider serviceProvider)
		{
			using (var context = new AppDbContext(
				serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
			{
				// Nếu đã có dữ liệu thì bỏ qua
				if (context.Categories.Any() || context.Products.Any())
				{
					return;
				}

				var categories = new List<Category>
				{
					new Category { Name = "Laptop" },
					new Category { Name = "Phone" },
					new Category { Name = "Accessory" },
					new Category { Name = "Tablet" }
				};
				context.Categories.AddRange(categories);
				context.SaveChanges();

				var products = new List<Product>
				{
					new Product { Name = "Laptop Dell XPS 13", Price = 25999000m, CategoryId = categories[0].Id },
					new Product { Name = "MacBook Pro M3", Price = 45999000m, CategoryId = categories[0].Id },
					new Product { Name = "iPhone 15 Pro Max", Price = 29999000m, CategoryId = categories[1].Id },
					new Product { Name = "Samsung Galaxy S24", Price = 22999000m, CategoryId = categories[1].Id },
					new Product { Name = "Sony WH-1000XM5", Price = 8999000m, CategoryId = categories[2].Id },
					new Product { Name = "Logitech MX Master 3S", Price = 2499000m, CategoryId = categories[2].Id },
					new Product { Name = "iPad Air", Price = 15999000m, CategoryId = categories[3].Id },
					new Product { Name = "LG UltraWide Monitor 34\"", Price = 12999000m, CategoryId = categories[0].Id }
				};

				context.Products.AddRange(products);
				context.SaveChanges();
			}
		}
	}
}
