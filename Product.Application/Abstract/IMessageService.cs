using Product.Domain.Entities;

namespace Product.Application.Abstract
{
    public interface IMessageService
    {
        Task<List<MessageClass>> GetAllAsync();
        Task<MessageClass?> GetByIdAsync(int id);
        Task<MessageClass> CreateAsync(MessageClass message);
    }
}
