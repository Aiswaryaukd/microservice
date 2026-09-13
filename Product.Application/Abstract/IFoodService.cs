using Product.Domain.Entities;

namespace Product.Application.Abstract
{
    public interface IFoodService
    {
        Task<List<FoodClass>> GetAllAsync();
        Task<FoodClass?> GetByIdAsync(int id);
        Task<FoodClass> CreateAsync(FoodClass food);
    }
}
