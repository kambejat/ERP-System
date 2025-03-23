using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models

{
    public class Supplier{
        [Key]
        [Column("supplier_id")]
        public int supplier_id { get; set; }
        [Required]
        [MaxLength(100)]
        [Column("name_of_supplier")]
        public string? name_of_supplier { get; set; }
        [Column("contact_person")]
        public string? contact_person { get; set; }
        [MaxLength(100)]
        [Column("email")]
        public string? email { get; set; }
        [MaxLength(20)]
        [Column("phone")]
        public string? phone { get; set; }
        [MaxLength(255)]
        [Column("address")]
        public string? address { get;  set; }
        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
    } 
}


