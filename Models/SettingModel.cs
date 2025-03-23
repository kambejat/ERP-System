using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class Setting
    {
        [Key]
        [Column("setting_key")]
        public string? setting_key { get; set; }

        [Required]
        [Column("setting_value")]
        public string? setting_value { get; set; }
    }
}
