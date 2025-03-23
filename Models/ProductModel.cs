
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int product_id { get; set; }

        [Required]
        [Column("name_of_product")]
        [MaxLength(100)]
        public string? name_of_product { get; set; }

        [Column("description")]
        public string? description { get; set; }

        [Column("sku")]
        [MaxLength(50)]
        public string? sku { get; set; }

        [Required]
        [Column("price", TypeName = "decimal(10, 2)")]
        public decimal price { get; set; }

        [Column("quantity_in_stock")]
        public int quantity_in_stock { get; set; } = 0;

        [Column("reorder_level")]
        public int reorder_level { get; set; } = 10;

        [Column("category")]
        [MaxLength(50)]
        public string? category { get; set; }

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime updated_at { get; set; } = DateTime.UtcNow;
    }
}
