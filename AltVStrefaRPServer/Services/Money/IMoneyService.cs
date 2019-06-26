using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Services.Money
{
    public interface IMoneyService
    {
        void GiveMoney(IMoney receiver, float amount);
        bool RemoveMoney(IMoney receiver, float amount);
        Task<bool> TransferMoneyFromEntityToEntity(IMoney source, IMoney receiver, float amount, TransactionType transactionType);
        Task<bool> TransferMoneyFromBankAccountToEntity(Character source, IMoney receiver, float amount, TransactionType transactionType);
    }
}
