namespace BankDataWebService.Models.Generator
{
    public class BankGene
    {
        private Random random = new Random();
        int index = 100000;

        private int GetAccountNumber()
        {
            index++;
            return index;
        }

        private double GetBalance()
        {
            return random.Next(0, 99999);
        }

        public Bank GetNextBank()
        {
            Bank b = new Bank();
            b.accountNumber = GetAccountNumber();
            b.balance = GetBalance();
            return b;
        }
    }
}
