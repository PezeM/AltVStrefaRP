using System.ComponentModel.DataAnnotations.Schema;

namespace AltVStrefaRPServer.Models
{
    public class BankAccount : IMoney
    {
        private float _money;

        public int Id { get; set; }
        public float Money
        {
            get { return _money; }
            set
            {
                _money = value;
                if(UpdateOnMoneyChange) OnMoneyChange();
            }
        }

        public int AccountNumber { get; set; }
        public Character Character { get; set; }
        public int CharacterId { get; set; }

        [NotMapped]
        public bool UpdateOnMoneyChange => false;

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

        public bool TransferMoney(IMoney receiver, float amount, float amountAfterTax)
        {
            if (Money < amountAfterTax) return false;
            Money -= amountAfterTax;
            receiver.Money += amount;
            return true;
        }

        public bool TransferMoney(IMoney receiver, float amount)
        {
            if (Money < amount) return false;
            Money -= amount;
            receiver.Money += amount;
            return true;
        }

        public override string ToString() => $"BankAccount {AccountNumber}";

        public string MoneyTransactionDisplayName()
        {
            return $"BankAccount {AccountNumber}";
        }

        public void OnMoneyChange() { }
    }
}
