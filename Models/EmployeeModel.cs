using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class Employee
    {
        [Key]
        [Column("employee_id")]
        public int employee_id { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int? user_id { get; set; }

        public User? User { get; set; }

        [Required]
        [Column("first_name")]
        public string? first_name { get; set; }

        [Required]
        [Column("last_name")]
        public string? last_name { get; set; }

        [Required]
        [Column("email")]
        public string? email { get; set; }

        [Column("phone")]
        public string? phone { get; set; }

        [Column("job_title")]
        public string? job_title { get; set; }

        [Column("department")]
        public string? department { get; set; }

        [Required]
        [Column("hire_date")]
        public DateTime hire_date { get; set; }

        [Column("salary")]
        [Range(0, double.MaxValue)]
        public decimal? salary { get; set; }

        [Required]
        [Column("status")]
        public string status { get; set; } = "active";

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime updated_at { get; set; } = DateTime.UtcNow;
    }
}
