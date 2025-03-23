using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class Attendance
    {
        [Key]
        [Column("attendance_id")]
        public int attendance_id { get; set; }

        [ForeignKey("Employee")]
        [Column("employee_id")]
        public int employee_id { get; set; }

        public Employee? Employee { get; set; }

        [Required]
        [Column("date")]
        public DateTime date { get; set; }

        [Column("check_in")]
        public DateTime? check_in { get; set; }

        [Column("check_out")]
        public DateTime? check_out { get; set; }

        [Required]
        [Column("status")]
        public string status { get; set; } = "present";

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime updated_at { get; set; } = DateTime.UtcNow;
    }
}
