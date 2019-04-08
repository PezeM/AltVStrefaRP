using System;

namespace AltVStrefaRPServer.Models
{
    public class MoneyTransaction
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Receiver { get; set; }
        public string Date { get; set; }
        public TransactionType Type { get; set; }
        public float Amount { get; set; }

        public MoneyTransaction(string source, string receiver, TransactionType type, float amount)
        {
            Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            Source = source;
            Receiver = receiver;
            Type = type;
            Amount = amount;
        }
    }
}
