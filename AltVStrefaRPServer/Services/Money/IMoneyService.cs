using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Money
{
    public interface IMoneyManager
    {
        bool GiveMoney(Character receiver, float amount);
    }
}
