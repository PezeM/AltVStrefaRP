using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Services.Money
{
    public interface IMoneyService
    {
        void GiveMoney(IMoney receiver, float amount);
        bool RemoveMoney(IMoney receiver, float amount);

        bool GiveMoney(Character receiver, float amount);
        bool RemoveMoney(Character receiver, float amount);
        Task<bool> RemoveMoneyFromBankAccountAsync(Character receiver, float amount, string source,
            TransactionType transactionType = TransactionType.Uncategorized);
        bool RemoveMoneyFromBankAccount(Character receiver, float amount, string source,
            TransactionType transactionType = TransactionType.Uncategorized);
        bool TransferMoneyToAnotherPlayer(Character source, Character receiver, float amount);
        Task<bool> TransferMoneyToAnotherPlayerAsync(Character source, Character receiver, float amount);
        bool WithdrawMoneyFromBankAccount(Character source, BankAccount bankAccount, float amount);
        Task<bool> WithdrawMoneyFromBankAccountAsync(Character source, BankAccount bankAccount, float amount);
        bool DepositMoneyToBankAccount(Character source, BankAccount bankAccount, float amount);
        Task<bool> DepositMoneyToBankAccountAsync(Character source, BankAccount bankAccount, float amount);
        bool TransferMoneyFromBankAccountToBankAccount(BankAccount sender, BankAccount receiver, float amount);
        Task<bool> TransferMoneyFromBankAccountToBankAccountAsync(BankAccount sender, BankAccount receiver, float amount);

        Task<bool> TransferMoneyFromBankAccountToEntity(Character source, IMoney receiver, float amount, TransactionType transactionType);
    }
}
