using Product.Domain.Entities;

namespace Product.Domain.Abstract
{
    public interface IMessageRepository
    {
        Task<IEnumerable<MessageClass>> GetAllAsync();
        Task<MessageClass?> GetByIdAsync(int id);
        Task<MessageClass> CreateAsync(MessageClass message);
    }
}
