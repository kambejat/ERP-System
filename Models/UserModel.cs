using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models

{
    public class User {

        [Key]
        [Column("user_id")]
        public int user_id { get; set; }
        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string? username { get; set; }
        [Required]
        [MaxLength(100)]
        [Column("email")]
        public string? email { get; set; }
        [Required]
        [MaxLength(255)]
        [Column("password_hash")]
        public string? password_hash { get; set; }
        [Required]
        [EnumDataType(typeof(UserRole))]
        [Column("role")]
        public string? role { get; set; }
        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime updated_at { get; set; } = DateTime.UtcNow;

    }

    public enum UserRole {
        Admin,
        Manager,
        Staff
    }
}
