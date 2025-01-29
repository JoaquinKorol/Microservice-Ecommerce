namespace Order.DTOs
{
    public class CreateOrderDTO
    {
       
        public int UserId { get; set; }

 
        public string ShippingAddress { get; set; }
        

  
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }

}
