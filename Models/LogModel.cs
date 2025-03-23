using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class Log
    {
        [Key]
        [Column("log_id")]
        public int log_id { get; set; }

        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public int user_id { get; set; }

        public User? User { get; set; }

        [Required]
        [Column("action")]
        public string? action { get; set; }

        [Column("timestamp")]
        public DateTime timestamp { get; set; } = DateTime.UtcNow;
    }
}
