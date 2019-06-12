namespace AltVStrefaRPServer.Models
{
    public interface IMoney
    {
        bool UpdateOnMoneyChange { get; }
        float Money { get; set; }
        string MoneyTransactionDisplayName();
        void OnMoneyChange();
    }
}
