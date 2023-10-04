using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Reflection.Metadata;

namespace BankDataWebService.Models
{
    public class Profile
    {
        [Key]
        public string? email { get; set; }
        public string? name { get; set; }
        public string? address { get; set; }
        public string? phone { get; set; }
        public string? picture { get; set; }
        public string? password { get; set; }
    }
}
