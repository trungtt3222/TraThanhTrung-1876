using CallAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Diagnostics;

namespace CallAPI.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IHttpClientFactory _httpClientFactory;

		public HomeController(
			ILogger<HomeController> logger,
			IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IActionResult> Index()
		{
			var client = _httpClientFactory.CreateClient("ProductApi");
			var products = await FetchAsync<List<ProductViewModel>>(client, "api/products") ?? new List<ProductViewModel>();
			var categories = await FetchAsync<List<CategoryViewModel>>(client, "api/categories") ?? new List<CategoryViewModel>();

			ViewBag.Categories = categories;
			return View(products);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		private static async Task<T?> FetchAsync<T>(HttpClient client, string url)
		{
			try
			{
				return await client.GetFromJsonAsync<T>(url);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Call API error: {ex.Message}");
				return default;
			}
		}
	}
}
