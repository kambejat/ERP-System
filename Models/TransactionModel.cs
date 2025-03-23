using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.Models
{
    public class Transaction
    {
        [Key]
        [Column("transaction_id")]
        public int transaction_id { get; set; }

        [Required]
        [ForeignKey("Order")]
        [Column("order_id")]
        public int order_id { get; set; }

        public Order? Order { get; set; }

        [Column("transaction_date")]
        public DateTime transaction_date { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("amount", TypeName = "decimal(12, 2)")]
        public decimal amount { get; set; }

        [Required]
        [Column("payment_method")]
        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod payment_method { get; set; }

        [Required]
        [Column("status")]
        [EnumDataType(typeof(TransactionStatus))]
        public TransactionStatus status { get; set; } = TransactionStatus.pending;
    }

    public enum PaymentMethod
    {
        cash,
        card,
        bank_transfer
    }

    public enum TransactionStatus
    {
        pending,
        completed,
        failed
    }
}
