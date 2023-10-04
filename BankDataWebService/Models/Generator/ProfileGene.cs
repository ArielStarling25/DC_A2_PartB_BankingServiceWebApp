﻿namespace BankDataWebService.Models.Generator
{
    public class ProfileGene
    {
        int index = 1;
        private Random random = new Random();
        private string[] names = { "Thomas", "Landy", "Jason", "Alex", "Aaron", "Adams", "Baker", "Clark", "Davis", "Evans", "Frank", "Ghosh", "Hills", "Irwin", "Jones", "Klein", "Lopez", "Mason", "Nalty", "Ochoa", "Patel", "Quinn", "Reily", "Smith", "Trott", "Usman", "Valdo", "White", "Xiang", "Yakub", "Zafar" };
        private string[] fileName = { "Prof1.jpg", "Prof2.jpg", "Prof3.jpg", "Prof4.png", "Prof5.png", "Prof6.png"};
        private string[] jalan = { "Jalan Annato", "Jalan Angsana", "Jalan Ansellia", "Jalan Asmara", "Jalan Bendahara", "Jalan Bintang", "Jalan Bintang Jaya 1", "Jalan Bintang Jaya 2", "Jalan Bintang Jaya 3", "Jalan Bintang Jaya 4", "Jalan Bintang Jaya 5, Jalan Pakis", "Jalan Permaisuri", "Jalan Merbau" };

        private string GetName()
        {
            return names[random.Next(0, names.Length)];
        }

        private string GetPassword()
        {
            return random.Next(1000, 9999).ToString();
        }

        private string GetAddress()
        {
            int num = random.Next(1, 999);
            string jalanStr = jalan[random.Next(0, jalan.Length)];
            return "No." + num + ", " + jalanStr + ", Miri, Sarawak.";
        }

        private string GetPhone()
        {
            int num = random.Next(1, 9999999);
            return "01" + num.ToString();
        }

        private string GetPicture()
        {
            byte[] file = File.ReadAllBytes(@"Pictures/" + fileName[random.Next(0, fileName.Length)]);
            return Convert.ToBase64String(file);
        }

        public Profile GetNextProfile()
        {
            Profile p = new Profile();
            p.name = GetName();
            p.phone = GetPhone();
            p.email = p.name + index.ToString() + "@gmail.com";
            p.address = GetAddress();
            p.picture = GetPicture();
            p.password = GetPassword();
            index++;
            return p;
        }
    }
}
