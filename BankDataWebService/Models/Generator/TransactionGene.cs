namespace BankDataWebService.Models.Generator
{
    public class TransactionGene
    {
        private Random random = new Random();
        int index = 100000;
        int id = 0;

        private int GetId()
        {
            id++;
            return id;
        }

        private int GetAccountNumber()
        {
            index++;
            return index;
        }

        private double GetAmount()
        {
            return random.Next(-99999, 99999);
        }

        public Transaction GetNextTransaction()
        {
            Transaction t = new Transaction();
            t.Id = GetId();
            t.accountNumber = GetAccountNumber();
            t.amount = GetAmount();
            return t;
        }
    }
}
