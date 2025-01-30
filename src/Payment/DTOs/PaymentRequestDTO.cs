namespace Payment.DTOs
{
    public class PaymentRequestDTO
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }

        public int OrderId { get; set; }
    }
}
