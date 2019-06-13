using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Services.Fractions;

namespace AltVStrefaRPServer.Services.Money
{
    public class MoneyService : IMoneyService
    {
        private readonly ServerContext _serverContext;
        private readonly ITaxService _taxService;

        public MoneyService(ServerContext serverContext, ITaxService taxService)
        {
            _serverContext = serverContext;
            _taxService = taxService;
        }

        /// <summary>
        /// Gives money to <see cref="IMoney"/>
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        public void GiveMoney(IMoney receiver, float amount)
        {
            receiver.Money += amount;
        }

        /// <summary>
        /// Removes money from <see cref="IMoney"/>
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool RemoveMoney(IMoney receiver, float amount)
        {
            if (receiver.Money < amount) return false;
            receiver.Money += amount;
            return true;
        }

        /// <summary>
        /// Transfers money from <see cref="IMoney"/> to <see cref="IMoney"/>
        /// </summary>
        /// <param name="source">Source of the money</param>
        /// <param name="receiver">Receiver of the money</param>
        /// <param name="amount">Amount of money to transfer</param>
        /// <param name="transactionType">Type of transaction</param>
        /// <returns></returns>
        public async Task<bool> TransferMoneyFromEntityToEntity(IMoney source, IMoney receiver, float amount, TransactionType transactionType)
        {
            amount = _taxService.CalculatePriceAfterTax(amount, transactionType);
            if (source.Money < amount) return false;
            source.Money -= amount;
            receiver.Money += amount;

            await SaveTransfer(source, receiver, amount, transactionType);
            return true;
        }

        public async Task<bool> TransferMoneyFromBankAccountToEntity(Character source, IMoney receiver, float amount, TransactionType transactionType)
        {
            amount = _taxService.CalculatePriceAfterTax(amount, transactionType);
            if (source.BankAccount == null) return false;
            else if (!source.BankAccount.TransferMoney(receiver, amount)) return false;

            await SaveTransfer(source, receiver, amount, transactionType);
            return true;
        }

        private async Task SaveTransfer(IMoney source, IMoney receiver, float amount, TransactionType transactionType)
        {
            await AddTransaction(source.MoneyTransactionDisplayName(), receiver.MoneyTransactionDisplayName(), amount, transactionType);
            //await _serverContext.MoneyTransactions.AddAsync(new MoneyTransaction(source.MoneyTransactionDisplayName(),
            //    receiver.MoneyTransactionDisplayName(), transactionType, amount));
            _serverContext.UpdateRange(source, receiver);
            await _serverContext.SaveChangesAsync();
        }

        private async Task SaveTransfer(Character source, IMoney receiver, float amount, TransactionType transactionType)
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
