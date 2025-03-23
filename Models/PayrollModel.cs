using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class Payroll
    {
        [Key]
        [Column("payroll_id")]
        public int payroll_id { get; set; }

        [ForeignKey("Employee")]
        [Column("employee_id")]
        public int employee_id { get; set; }

        public Employee? Employee { get; set; }

        [Required]
        [Column("pay_period_start")]
        public DateTime pay_period_start { get; set; }

        [Required]
        [Column("pay_period_end")]
        public DateTime pay_period_end { get; set; }

        [Required]
        [Column("gross_salary")]
        public decimal gross_salary { get; set; }

        [Column("tax")]
        public decimal tax { get; set; } = 0;

        [NotMapped]
        [Column("net_salary")]
        public decimal net_salary => gross_salary - tax;

        [Required]
        [Column("status")]
        public string status { get; set; } = "pending";

        [Column("processed_at")]
        public DateTime processed_at { get; set; } = DateTime.UtcNow;
    }
}
