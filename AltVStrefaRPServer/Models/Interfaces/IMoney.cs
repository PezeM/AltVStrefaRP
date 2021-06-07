namespace AltVStrefaRPServer.Models.Interfaces
{
    public interface IMoney
    {
        float Money { get; }
        void AddMoney(float amount);
        bool RemoveMoney(float amount);
        string MoneyTransactionDisplayName();
    }
}
