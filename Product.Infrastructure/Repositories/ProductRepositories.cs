using Microsoft.EntityFrameworkCore;
using Product.Domain.Abstract;
using Product.Domain.Entities;
using Product.Infrastructure.Data;

namespace Product.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductClass>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<ProductClass?> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProductClass> CreateAsync(ProductClass product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }

}
