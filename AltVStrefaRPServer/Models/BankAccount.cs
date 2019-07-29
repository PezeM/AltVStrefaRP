using AltVStrefaRPServer.Services;

namespace AltVStrefaRPServer.Models
{
    public class BankAccount : IMoney
    {
        public int Id { get; set; }
        public float Money { get; set; }
        public bool ShowNotificationOnMoneyTransfer { get; set; } = true;

        public int AccountNumber { get; set; }
        public Character Character { get; set; }
        public int CharacterId { get; set; }

        public void AddMoney(float amount)
        {
            Money += amount;
        }

        public bool RemoveMoney(float amount)
        {
            if (Money < amount) return false;
            Money -= amount;
            return true;
        }

        public bool TransferMoney(IMoney receiver, float amount, float amountAfterTax)
        {
            if (RemoveMoney(amountAfterTax)) return false;
            receiver.AddMoney(amount);
            return true;
        }

        public bool TransferMoney(IMoney receiver, float amount)
        {
            if (RemoveMoney(amount)) return false;
            receiver.AddMoney(amount);
            return true;
        }

        public override string ToString() => $"BankAccount {AccountNumber}";

        public string MoneyTransactionDisplayName()
        {
            return $"BankAccount {AccountNumber}";
        }

        public void NotifyOnMoneyTransfer(IMoney source, int money, INotificationService notificationService)
        {
            if (!ShowNotificationOnMoneyTransfer) return;
            if (Character == null || Character.Player == null) return;
            notificationService.ShowSuccessNotification(Character.Player, "Otrzymano przelew",
                $"Właśnie otrzymałeś przelew od {source.MoneyTransactionDisplayName()} w wysokości {money}$. <br>" +
                $"Twój aktualny stan konta wynosi {Money}$", 7000);
        }
    }
}
