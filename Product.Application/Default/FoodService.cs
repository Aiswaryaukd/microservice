using Product.Application.Abstract;
using Product.Domain.Abstract;
using Product.Domain.Entities;

namespace Product.Application.Default
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _repository;

        public FoodService(IFoodRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FoodClass>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.ToList();
        }

        public async Task<FoodClass?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<FoodClass> CreateAsync(FoodClass food)
        {
            return await _repository.CreateAsync(food);
        }
    }
}
