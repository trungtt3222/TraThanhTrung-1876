using ProductApi.Dto;

namespace ProductApi.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<(ProductDto? product, string? errorCode, string? message)> CreateAsync(CreateProductDto dto);
        Task<(ProductDto? product, string? errorCode, string? message)> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

