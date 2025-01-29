namespace Order.DTOs
{
    public class UpdateOrderDTO
    {
        
        public string ShippingAddress { get; set; }

        
        public decimal TotalAmount { get; set; }

       
        public int Status { get; set; }

        
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }
}
