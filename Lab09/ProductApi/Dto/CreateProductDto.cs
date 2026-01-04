using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProductApi.Dto
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public IFormFile? Image { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
