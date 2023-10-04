using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace BankDataWebService.Models
{
    public class Bank
    {
        [Key]
        public int accountNumber { get; set; }

        public double balance { get; set; }

        public string? email { get; set; }
    }
}
