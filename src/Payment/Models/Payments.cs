namespace Payment.Models
{
    public class Payments
    {
        public int Id { get; set; }
        public int OrderId { get; set; } 
        public string PaymentStatus { get; set; } 
        public string PaymentMethod { get; set; } 
        public string Amount { get; set; } 
        public DateTime CreatedAt { get; set; }
        public long MercadoPagoPaymentId { get; set; } 
    }
}
