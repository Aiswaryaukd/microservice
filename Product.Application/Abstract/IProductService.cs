using Product.Application.DTO;
using Product.Domain.Entities;

namespace Product.Application.Abstract
{
    public interface IProductService
    {
        Task<List<ProductClass>> GetAllAsync();
        Task<ProductClass?> GetByIdAsync(int id);
        Task<ProductClass> CreateAsync(ProductClass product);
    }
}
