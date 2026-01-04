using ProductApi.Dto;
using ProductApi.Models;
using ProductApi.Repositories;

namespace ProductApi.Services
{
    public class ProductService : IProductService
    {
        private static readonly string[] AllowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _environment;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IWebHostEnvironment environment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _environment = environment;
        }

        public async Task<(ProductDto? product, string? errorCode, string? message)> CreateAsync(CreateProductDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return (null, "CATEGORY_NOT_FOUND", "Category does not exist");
            }

            if (dto.Image == null)
            {
                return (null, "IMAGE_REQUIRED", "Image is required");
            }

            var saveResult = await SaveFileAsync(dto.Image);
            if (!saveResult.success)
            {
                return (null, saveResult.errorCode, saveResult.message);
            }

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                ImageUrl = saveResult.relativePath
            };

            product = await _productRepository.AddAsync(product);
            product.Category = category;

            return (MapToDto(product), null, null);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            await _productRepository.DeleteAsync(product);

            DeletePhysicalFile(product.ImageUrl);

            return true;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : MapToDto(product);
        }

        public async Task<(ProductDto? product, string? errorCode, string? message)> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return (null, "PRODUCT_NOT_FOUND", "Product does not exist");
            }

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return (null, "CATEGORY_NOT_FOUND", "Category does not exist");
            }

            string? newImagePath = null;
            if (dto.Image != null)
            {
                var saveResult = await SaveFileAsync(dto.Image);
                if (!saveResult.success)
                {
                    return (null, saveResult.errorCode, saveResult.message);
                }

                newImagePath = saveResult.relativePath;
            }

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            if (newImagePath != null)
            {
                DeletePhysicalFile(product.ImageUrl);
                product.ImageUrl = newImagePath;
            }

            await _productRepository.UpdateAsync(product);
            product.Category = category;

            return (MapToDto(product), null, null);
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name
            };
        }

        private (string? fullPath, string? relativePath, bool success, string? errorCode, string? message) SavePath(string fileName)
        {
            var webRoot = string.IsNullOrWhiteSpace(_environment.WebRootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : _environment.WebRootPath;

            var uploadFolder = Path.Combine(webRoot, "uploads");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var relativePath = Path.Combine("uploads", fileName).Replace("\\", "/");
            var fullPath = Path.Combine(uploadFolder, fileName);

            return (fullPath, "/" + relativePath, true, null, null);
        }

        private async Task<(bool success, string? relativePath, string? errorCode, string? message)> SaveFileAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return (false, null, "INVALID_FILE_TYPE", "Only jpg, jpeg or png are allowed");
            }

            var newFileName = $"{Guid.NewGuid()}{extension}";
            var pathInfo = SavePath(newFileName);

            if (!pathInfo.success || pathInfo.fullPath == null || pathInfo.relativePath == null)
            {
                return (false, null, "SYSTEM_ERROR", "Unable to build upload path");
            }

            using (var stream = new FileStream(pathInfo.fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, pathInfo.relativePath, null, null);
        }

        private void DeletePhysicalFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            var webRoot = string.IsNullOrWhiteSpace(_environment.WebRootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : _environment.WebRootPath;

            var fullPath = Path.Combine(webRoot, relativePath.TrimStart('/')).Replace("/", Path.DirectorySeparatorChar.ToString());

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}

