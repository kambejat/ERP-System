using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class LeaveRequest
    {
        [Key]
        [Column("leave_id")]
        public int leave_id { get; set; }

        [ForeignKey("Employee")]
        [Column("employee_id")]
        public int employee_id { get; set; }

        public Employee? Employee { get; set; }

        [Required]
        [Column("start_date")]
        public DateTime start_date { get; set; }

        [Required]
        [Column("end_date")]
        public DateTime end_date { get; set; }

        [Column("leave_type")]
        public string? leave_type { get; set; }

        [Required]
        [Column("status")]
        public string status { get; set; } = "pending";

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime updated_at { get; set; } = DateTime.UtcNow;
    }
}
