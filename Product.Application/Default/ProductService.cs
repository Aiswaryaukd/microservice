using Product.Application.Abstract;
using Product.Domain.Entities;
using Product.Domain.Abstract;

namespace Product.Application.Default
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductClass>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.ToList();
        }

        public async Task<ProductClass?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ProductClass> CreateAsync(ProductClass product)
        {
            return await _repository.CreateAsync(product);
        }
    }

}
