using Product.Domain.Entities;

namespace Product.Domain.Abstract
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductClass>> GetAllAsync();
        Task<ProductClass?> GetByIdAsync(int id);
        Task<ProductClass> CreateAsync(ProductClass product);
    }

}
