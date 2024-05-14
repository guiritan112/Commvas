using System;
using System.ComponentModel.DataAnnotations;

namespace ThisCommVas.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
