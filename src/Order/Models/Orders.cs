using System.ComponentModel.DataAnnotations;

namespace Order.Models
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }                // Identificador único de la orden
        public int UserId { get; set; }            // ID del usuario (relación con la API de Usuarios)
        public DateTime OrderDate { get; set; }    // Fecha en que se realizó la orden
        public string ShippingAddress { get; set; } 
        public decimal TotalAmount { get; set; }   // Monto total del pedido
        public int StatusId { get; set; }            // Estado de la orden (se relaciona con OrderStatus)

        // Relación de 1:N con OrderItems
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
