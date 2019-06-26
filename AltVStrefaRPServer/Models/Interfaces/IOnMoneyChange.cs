namespace AltVStrefaRPServer.Models
{
    public interface IOnMoneyChange
    {
        bool UpdateOnMoneyChange { get; set; }
        void OnMoneyChange();
    }
}
