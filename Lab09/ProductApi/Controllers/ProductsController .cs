using Microsoft.AspNetCore.Mvc;
using ProductApi.Dto;
using ProductApi.Services;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { errorCode = "PRODUCT_NOT_FOUND", message = "Product not found" });
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productService.CreateAsync(dto);
            if (result.errorCode != null)
            {
                return BadRequest(new { errorCode = result.errorCode, message = result.message });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.product!.Id }, result.product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productService.UpdateAsync(id, dto);
            if (result.errorCode != null)
            {
                if (result.errorCode == "PRODUCT_NOT_FOUND")
                {
                    return NotFound(new { errorCode = result.errorCode, message = result.message });
                }
                return BadRequest(new { errorCode = result.errorCode, message = result.message });
            }

            return Ok(result.product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { errorCode = "PRODUCT_NOT_FOUND", message = "Product not found" });
            }

            return NoContent();
        }
    }
}
