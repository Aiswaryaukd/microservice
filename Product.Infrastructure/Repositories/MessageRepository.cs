using Microsoft.EntityFrameworkCore;
using Product.Domain.Abstract;
using Product.Domain.Entities;
using Product.Infrastructure.Data;

namespace Product.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ProductDbContext _context;

        public MessageRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MessageClass>> GetAllAsync()
        {
            return await _context.Messages
                .OrderByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<MessageClass?> GetByIdAsync(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MessageClass> CreateAsync(MessageClass message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }
    }
}
