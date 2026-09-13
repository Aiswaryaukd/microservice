using Product.Application.Abstract;
using Product.Domain.Abstract;
using Product.Domain.Entities;

namespace Product.Application.Default
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;

        public MessageService(IMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MessageClass>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.ToList();
        }

        public async Task<MessageClass?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<MessageClass> CreateAsync(MessageClass message)
        {
            return await _repository.CreateAsync(message);
        }
    }
}
