using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Order.Data;
using Order.Models;

namespace Order.Repositories
{
    public class OrderItemRepository : IRepository<OrderItem>
    {
        private readonly OrderContext _context;
        public OrderItemRepository(OrderContext context)
        {
            _context = context;
        }  
        public async Task AddAsync(OrderItem entity)
        {
            await _context.Set<OrderItem>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
           var orderItem = _context.Set<OrderItem>().Find(id);
            if (orderItem != null)
            {
                _context.Set<OrderItem>().Remove(orderItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            return await _context.Set<OrderItem>().ToListAsync();
        }

        public async Task<OrderItem> GetByIdAsync(int id)
        {
            return await _context.Set<OrderItem>().FindAsync(id);
        }

        public async Task UpdateAsync(OrderItem orderItem)
        {
            _context.Set<OrderItem>().Update(orderItem);
            await _context.SaveChangesAsync();
        }
    }
}
