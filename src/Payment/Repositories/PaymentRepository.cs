using Core.Interfaces;
using MercadoPago.Resource.Payment;
using Microsoft.EntityFrameworkCore;
using Payment.Data;
using Payment.Models;
using System.Drawing.Printing;

namespace Payment.Repositories
{
    public class PaymentRepository : IRepository<Payments>
    {

        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Payments payment)
        {
            await _context.Set<Payments>().AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var payment = _context.Set<Payments>().Find(id);
            if (payment != null)
            {
                _context.Set<Payments>().Remove(payment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Payments>> GetAllAsync()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<Payments> GetByIdAsync(int id)
        {
            return await _context.Set<Payments>().FindAsync(id);
        }

        public async Task UpdateAsync(Payments entity)
        {
            _context.Set<Payments>().Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
