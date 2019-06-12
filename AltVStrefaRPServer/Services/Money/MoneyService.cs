using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Services.Money
{
    public class MoneyService : IMoneyService
    {
        private readonly ServerContext _serverContext;

        public MoneyService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        public void GiveMoney(IMoney receiver, float amount)
        {
            receiver.Money += amount;
        }

        public bool RemoveMoney(IMoney receiver, float amount)
        {
            if (receiver.Money < amount) return false;
            receiver.Money += amount;
            return true;
        }

        public async Task<bool> TransferMoneyFromEntityToEntity(IMoney source, IMoney receiver, float amount, TransactionType transactionType)
        {
            if (source.Money < amount) return false;
            source.Money -= amount;
            receiver.Money += amount;

            await TransferMoney(source, receiver, amount, transactionType);
            return true;
        }

        public async Task<bool> TransferMoneyFromBankAccountToEntity(Character source, IMoney receiver, float amount, TransactionType transactionType)
        {
            if (source.BankAccount == null) return false;
            else if (!source.BankAccount.TransferMoney(receiver, amount)) return false;

            await TransferMoney(source, receiver, amount, transactionType);
            return true;
        }

        private async Task TransferMoney(IMoney source, IMoney receiver, float amount, TransactionType transactionType)
        {
            await AddTransaction(source.MoneyTransactionDisplayName(), receiver.MoneyTransactionDisplayName(), amount, transactionType);
            //await _serverContext.MoneyTransactions.AddAsync(new MoneyTransaction(source.MoneyTransactionDisplayName(),
            //    receiver.MoneyTransactionDisplayName(), transactionType, amount));
            _serverContext.UpdateRange(source, receiver);
            await _serverContext.SaveChangesAsync();
        }

        private async Task TransferMoney(Character source, IMoney receiver, float amount, TransactionType transactionType)
        {
            await AddTransaction(source.MoneyTransactionDisplayName(), receiver.MoneyTransactionDisplayName(), amount, transactionType);
            _serverContext.Characters.Update(source);
            _serverContext.Update(receiver);
            await _serverContext.SaveChangesAsync();
        }

        private Task AddTransaction(string source, string receiver, float amount, TransactionType transactionType)
        {
            return _serverContext.MoneyTransactions.AddAsync(new MoneyTransaction(source, receiver, transactionType, amount));
        }
    }
}
