using System;
using System.Threading.Tasks;
using AltV.Net;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Interfaces;

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
        public void GiveMoney(IMoney receiver, float amount) => receiver.AddMoney(amount);

        /// <summary>
        /// Removes money from <see cref="IMoney"/>
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool RemoveMoney(IMoney receiver, float amount) => receiver.RemoveMoney(amount);

        /// <summary>
        /// Transfers money from <see cref="IMoney"/> to <see cref="IMoney"/>
        /// </summary>
        /// <param name="source">Source of the money</param>
        /// <param name="receiver">Receiver of the money</param>
        /// <param name="amount">Amount of money to transfer</param>
        /// <param name="transactionType">Type of transaction</param>
        /// <returns></returns>
        public async Task<bool> TransferMoneyFromEntityToEntityAsync(IMoney source, IMoney receiver, float amount, TransactionType transactionType)
        {
            var afterTax = _taxService.CalculatePriceAfterTax(amount, transactionType);
            if (!source.RemoveMoney(afterTax)) return false;
            receiver.AddMoney(amount);
            await SaveTransferAsync(source, receiver, amount, transactionType);
            return true;
        }

        public async Task<bool> TransferMoneyFromBankAccountToEntityAsync(Character source, IMoney receiver, float amount, TransactionType transactionType)
        {
            var afterTax = _taxService.CalculatePriceAfterTax(amount, transactionType);
            if (source.BankAccount == null) return false;
            else if (!source.BankAccount.TransferMoney(receiver, amount, afterTax)) return false;

            await SaveTransferAsync(source, receiver, amount, transactionType);
            return true;
        }

        private async Task SaveTransferAsync(IMoney source, IMoney receiver, float amount, TransactionType transactionType)
        {
            using (var context = _factory.Invoke())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await AddMoneyTransactionAsync(context,
                            new MoneyTransaction(source.MoneyTransactionDisplayName(), receiver.MoneyTransactionDisplayName(), transactionType, amount));
                        context.UpdateRange(source, receiver);
                        await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Alt.Server.LogError($"Error in saving money transfer. Transaction rolled back. Transaction type: {transactionType} amount: {amount}. " +
                                            $"Error: {e}");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private async Task SaveTransferAsync(Character source, IMoney receiver, float amount, TransactionType transactionType)
        {
            using (var context = _factory.Invoke())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await AddMoneyTransactionAsync(context,
                            new MoneyTransaction(source.MoneyTransactionDisplayName(), receiver.MoneyTransactionDisplayName(), transactionType, amount));
                        context.Characters.Update(source);
                        context.Update(receiver);
                        await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Alt.Server.LogError($"Error in saving money transfer. Transaction rolled back. " +
                                            $"Character: {source.Id} Transaction type: {transactionType} amount: {amount}. " +
                                            $"Error: {e}");
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void AddMoneyTransaction(ServerContext context, MoneyTransaction moneyTransaction)
        {
            context.MoneyTransactions.Add(moneyTransaction);
        }

        private async Task AddMoneyTransactionAsync(ServerContext context, MoneyTransaction moneyTransaction)
        {
            await context.MoneyTransactions.AddAsync(moneyTransaction);
        }
    }
}
