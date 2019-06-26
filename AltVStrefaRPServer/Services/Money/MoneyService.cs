using System;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Services.Money
{
    public class MoneyService : IMoneyService
    {
        private readonly Func<ServerContext> _factory;
        private readonly ITaxService _taxService;

        public MoneyService(Func<ServerContext> factory, ITaxService taxService)
        {
            _factory = factory;
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
            var afterTax = _taxService.CalculatePriceAfterTax(amount, transactionType);
            if (source.Money < afterTax) return false;
            source.Money -= afterTax;
            receiver.Money += amount;

            await SaveTransfer(source, receiver, amount, transactionType);
            return true;
        }

        public async Task<bool> TransferMoneyFromBankAccountToEntity(Character source, IMoney receiver, float amount, TransactionType transactionType)
        {
            var afterTax = _taxService.CalculatePriceAfterTax(amount, transactionType);
            if (source.BankAccount == null) return false;
            else if (!source.BankAccount.TransferMoney(receiver, amount, afterTax)) return false;

            await SaveTransfer(source, receiver, amount, transactionType);
            return true;
        }

        private async Task SaveTransfer(IMoney source, IMoney receiver, float amount, TransactionType transactionType)
        {
            using (var context = _factory.Invoke())
            {
                await AddTransaction(context,
                    new MoneyTransaction(source.MoneyTransactionDisplayName(), receiver.MoneyTransactionDisplayName(), transactionType, amount));
                context.UpdateRange(source, receiver);
                await context.SaveChangesAsync();
            }
        }

        private async Task SaveTransfer(Character source, IMoney receiver, float amount, TransactionType transactionType)
        {
            using (var context = _factory.Invoke())
            {
                await AddTransaction(context,
                    new MoneyTransaction(source.MoneyTransactionDisplayName(), receiver.MoneyTransactionDisplayName(), transactionType, amount));
                context.Characters.Update(source);
                context.Update(receiver);
                await context.SaveChangesAsync();
            }
        }

        private async Task AddTransaction(ServerContext context, MoneyTransaction moneyTransaction)
        {
            await context.MoneyTransactions.AddAsync(moneyTransaction);
        }
    }
}
