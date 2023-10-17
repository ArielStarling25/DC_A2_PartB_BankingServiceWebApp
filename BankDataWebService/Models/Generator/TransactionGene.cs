namespace BankDataWebService.Models.Generator
{
    public class TransactionGene
    {
        private Random random = new Random();
        int id = 0;
        int max;
        int index = 100000;

        public TransactionGene(int max)
        {
            this.max += max;
        }

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

        private int GetToAccountNumber()
        {
            int toReturn = index +1;
            if (toReturn == max)
            {
                toReturn = 100000;
            }
            return toReturn;
        }

        private double GetAmount()
        {
            return random.Next(1, 99999);
        }

        public Transaction GetNextTransaction()
        {
            Transaction t = new Transaction();
            t.Id = GetId();
            t.accountNumber = GetAccountNumber();
            t.toAccountNumber = GetToAccountNumber();
            t.amount = GetAmount();
            t.description = "";
            return t;
        }
    }
}
