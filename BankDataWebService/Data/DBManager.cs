using BankDataWebService.Models;
using BankDataWebService.Models.Generator;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace BankDataWebService.Data
{
    public class DBManager : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source = Banks.db;");
        }

        public DbSet<Bank> Banks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<Bank> banks = new List<Bank>();
            List<Profile> profiles = new List<Profile>();
            List<Transaction> transactions = new List<Transaction>();

            BankGene bankGene = new BankGene();
            ProfileGene profileGene = new ProfileGene();
            TransactionGene transactionGene = new TransactionGene();
            for (int i = 0; i < 50; i++)
            {
                Profile tempP = profileGene.GetNextProfile();
                Bank tempB = bankGene.GetNextBank();
                Transaction tempT = transactionGene.GetNextTransaction();
                tempB.email = tempP.email;
                profiles.Add(tempP);
                banks.Add(tempB);
                transactions.Add(tempT);
            }

            modelBuilder.Entity<Bank>().HasData(banks);
            modelBuilder.Entity<Transaction>().HasData(transactions);
            modelBuilder.Entity<Profile>().HasData(profiles);
        }

        public DbSet<Profile>? Profile { get; set; }

        public DbSet<Transaction>? Transaction { get; set; }
    }
}
