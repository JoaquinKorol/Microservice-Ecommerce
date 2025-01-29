namespace Order.DTOs
{
    public class CreateOrderDTO
    {
        // ID del usuario que está creando la orden
        public int UserId { get; set; }

        // Dirección de envío
        public string ShippingAddress { get; set; }
        
 
        // Monto total de la orden
        public decimal TotalAmount { get; set; }

        // Estado de la orden (esto puede ser un valor numérico que se relacione con una tabla de estados)
        public int Status { get; set; }

        // Lista de productos en la orden
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }

}
