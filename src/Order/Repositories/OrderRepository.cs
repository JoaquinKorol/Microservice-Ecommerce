using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Order.Data;
using Order.Models;
using System.Runtime.CompilerServices;

namespace Order.Repositories
{
    public class OrderRepository : IRepository<Orders>
    {
        private readonly OrderContext _context;
        public OrderRepository(OrderContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Orders entity)
        {
            await _context.Set<Orders>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = _context.Set<Orders>().Find(id);
            if (order != null)
            {
                _context.Set<Orders>().Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Orders>> GetAllAsync()
        {
            return await _context.Orders.Include(o => o.OrderItems).ToListAsync();
        }

        public async Task<Orders> GetByIdAsync(int id)
        {
            return await _context.Set<Orders>().FindAsync(id);
        }

        public async Task UpdateAsync(Orders order)
        {
            _context.Set<Orders>().Update(order);
            await _context.SaveChangesAsync();

        }

    }
}
