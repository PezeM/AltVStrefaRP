namespace AltVStrefaRPServer.Models
{
    public interface IMoney
    {
        float Money { get; set; }
        string MoneyTransactionDisplayName();
    }
}
