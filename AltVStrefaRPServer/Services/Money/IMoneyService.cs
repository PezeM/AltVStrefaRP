using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Interfaces;

namespace AltVStrefaRPServer.Services.Money
{
    public interface IMoneyService
    {
        void GiveMoney(IMoney receiver, float amount);
        bool RemoveMoney(IMoney receiver, float amount);
        Task<bool> TransferMoneyFromEntityToEntityAsync(IMoney source, IMoney receiver, float amount, TransactionType transactionType);
        Task<bool> TransferMoneyFromBankAccountToEntityAsync(Character source, IMoney receiver, float amount, TransactionType transactionType);
    }
}
