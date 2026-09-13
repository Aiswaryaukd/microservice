using Microsoft.EntityFrameworkCore;
using Product.Domain.Abstract;
using Product.Domain.Entities;
using Product.Infrastructure.Data;

namespace Product.Infrastructure.Repositories
{
    public class FoodRepository : IFoodRepository
    {
        private readonly ProductDbContext _context;

        public FoodRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FoodClass>> GetAllAsync()
        {
            return await _context.Foods.OrderBy(f => f.Id).ToListAsync();
        }

        public async Task<FoodClass?> GetByIdAsync(int id)
        {
            return await _context.Foods.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FoodClass> CreateAsync(FoodClass food)
        {
            await _context.Foods.AddAsync(food);
            await _context.SaveChangesAsync();
            return food;
        }
    }
}
