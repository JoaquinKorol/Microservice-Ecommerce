using Microsoft.EntityFrameworkCore;

namespace Payment.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }
        public DbSet<Payment.Models.Payments> Payments { get; set; }
    }
}
