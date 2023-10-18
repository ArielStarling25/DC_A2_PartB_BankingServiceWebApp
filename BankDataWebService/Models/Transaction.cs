using System.ComponentModel.DataAnnotations;

namespace BankDataWebService.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public int accountNumber { get; set; }
        public int toAccountNumber { get; set; }
        public double amount { get; set; }
        public string description { get; set; }
    }
}
