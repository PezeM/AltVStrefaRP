namespace AltVStrefaRPServer.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public float Money { get; set; }
        public int AccountNumber { get; set; }
        public Character Character { get; set; }
        public int CharacterId { get; set; }

        public bool DepositMoney(float amount)
        {
            Money += amount;
            return true;
        }

        public bool WithdrawMoney(float amount)
        {
            if (Money < amount) return false;
            Money -= amount;
            return true;
        }

        public bool TransferMoney(BankAccount receiver, float amount)
        {
            if (Money < amount) return false;
            receiver.Money += amount;
            Money -= amount;
            return true;
        }

        public override string ToString() => $"BankAccount {AccountNumber}";
    }
}
