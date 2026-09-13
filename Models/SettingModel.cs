using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class Setting
    {
        [Key]
        [Column("setting_key")]
        [Required]
        public string setting_key { get; set; } = string.Empty;

        [Required]
        [Column("setting_value")]
        public string setting_value { get; set; } = string.Empty;
    }
}