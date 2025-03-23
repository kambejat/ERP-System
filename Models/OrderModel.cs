using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int order_id { get; set; }

        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public int user_id { get; set; }

        public Erp.Models.User? User { get; set; }

        [Column("order_date")]
        public DateTime order_date { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("status")]
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus status { get; set; } = OrderStatus.pending;

        [Column("total_amount", TypeName = "decimal(12, 2)")]
        public decimal total_amount { get; set; } = 0;

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime updated_at { get; set; } = DateTime.UtcNow;

        public List<OrderItem> order_items { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        pending,
        completed,
        cancelled
    }
     public class OrderItem
    {
        [Key]
        [Column("order_item_id")]
        public int order_item_id { get; set; }

        [Required]
        [ForeignKey("Order")]
        [Column("order_id")]
        public int order_id { get; set; }

        public Order? Order { get; set; }

        [Required]
        [ForeignKey("Product")]
        [Column("product_id")]
        public int product_id { get; set; }

        public Erp.Models.Product? Product { get; set; }

        [Required]
        [Column("quantity")]
        public int quantity { get; set; }

        [Required]
        [Column("price", TypeName = "decimal(10, 2)")]
        public decimal price { get; set; }
    }
}
