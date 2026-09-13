using Product.Domain.Entities;

namespace Product.Domain.Abstract
{
    public interface IFoodRepository
    {
        Task<IEnumerable<FoodClass>> GetAllAsync();
        Task<FoodClass?> GetByIdAsync(int id);
        Task<FoodClass> CreateAsync(FoodClass food);
    }
}
