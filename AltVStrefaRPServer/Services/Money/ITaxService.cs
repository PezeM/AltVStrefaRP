using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions;

namespace AltVStrefaRPServer.Services.Money
{
    public interface ITaxService
    {
        float CalculateTax(float price, TransactionType transactionType, out TownHallFraction townHall);
    }
}
