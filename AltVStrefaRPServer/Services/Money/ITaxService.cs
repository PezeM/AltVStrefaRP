using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Services.Money
{
    public interface ITaxService
    {
        float CalculatePriceAfterTax(float price, TransactionType transactionType);
    }
}
