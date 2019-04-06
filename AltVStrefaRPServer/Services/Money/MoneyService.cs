﻿using System.Threading.Tasks;
using AltV.Net;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Money
{
    public class MoneyService : IMoneyService
    {
        private readonly ServerContext _serverContext;

        public MoneyService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        /// <summary>
        /// Gives source a certain amount of money
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool GiveMoney(Character receiver, float amount)
        {
            receiver.Money += amount;
            receiver.Player.SetSyncedMetaData("money", receiver.Money);
            return true;
        }

        /// <summary>
        /// Removes certain amount of money from source
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns>True if source had enough money</returns>
        public bool RemoveMoney(Character receiver, float amount)
        {
            if (receiver.Money < amount) return false;
            receiver.Money -= amount;
            return true;
        }

        /// <summary>
        /// Transfers money from one source to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns>True if source source had enough money to transfer</returns>
        public bool TransferMoneyToAnotherPlayer(Character source, Character receiver, float amount)
        {
            if (source.Money < amount) return false;
            source.Money -= amount;
            receiver.Money += amount;
            receiver.Player.SetSyncedMetaData("money", receiver.Money);
            source.Player.SetSyncedMetaData("money", source.Money);
            LogMoneyTransaction(source.Id, receiver.Id, TransactionType.PlayerToPlayer, amount);
            return true;
        }

        /// <summary>
        /// Transfers money from one source to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns>True if source source had enough money to transfer</returns>
        public async Task<bool> TransferMoneyToAnotherPlayerAsync(Character source, Character receiver, float amount)
        {
            if (source.Money < amount) return false;
            source.Money -= amount;
            receiver.Money += amount;
            receiver.Player.SetSyncedMetaData("money", receiver.Money);
            source.Player.SetSyncedMetaData("money", source.Money);
            await LogMoneyTransactionAsync(source.Id, receiver.Id, TransactionType.PlayerToPlayer, amount).ConfigureAwait(false);
            return true;
        }

        public bool WithdrawMoneyFromBankAccount(Character source, BankAccount bankAccount, float amount)
        {
            if (!bankAccount.WithdrawMoney(amount)) return false;
            source.Money += amount;
            source.Player.SetSyncedMetaData("money", source.Money);
            LogMoneyTransaction(source.Id, bankAccount.AccountNumber, TransactionType.BankWithdraw, amount);
            return true;
        }

        public async Task<bool> WithdrawMoneyFromBankAccountAsync(Character source, BankAccount bankAccount, float amount)
        {
            if (!bankAccount.WithdrawMoney(amount)) return false;
            source.Money += amount;
            source.Player.SetSyncedMetaData("money", source.Money);
            await LogMoneyTransactionAsync(source.Id, bankAccount.AccountNumber, TransactionType.BankWithdraw, amount).ConfigureAwait(false);
            return true;
        }

        public bool DepositMoneyToBankAccount(Character source, BankAccount bankAccount, float amount)
        {
            if (source.Money < amount) return false;
            bankAccount.DepositMoney(amount);
            source.Money -= amount;
            source.Player.SetSyncedMetaData("money", source.Money);
            LogMoneyTransaction(source.Id, bankAccount.AccountNumber, TransactionType.BankDeposit, amount);
            return true;
        }

        public async Task<bool> DepositMoneyToBankAccountAsync(Character source, BankAccount bankAccount, float amount)
        {
            if (source.Money < amount) return false;
            bankAccount.DepositMoney(amount);
            source.Money -= amount;
            source.Player.SetSyncedMetaData("money", source.Money);
            await LogMoneyTransactionAsync(source.Id, bankAccount.AccountNumber, TransactionType.BankDeposit, amount).ConfigureAwait(false);
            return true;
        }

        public bool TransferMoneyFromBankAccountToBankAccount(BankAccount sender, BankAccount receiver, float amount)
        {
            if (receiver.AccountNumber < 1) return false;
            if (!sender.TransferMoney(receiver, amount)) return false;
            SaveBankAccounts(new BankAccount[]{sender,receiver});
            LogMoneyTransaction(sender.Id, receiver.Id, TransactionType.BankTransfer, amount);
            return true;
        }

        /// <summary>
        /// Transfers money from one bank account to another
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        /// <returns>Returns true if the transaction was successful,
        /// false if the receiver didn't have bank account or sender didn't have enough money</returns>
        public async Task<bool> TransferMoneyFromBankAccountToBankAccountAsync(BankAccount sender, BankAccount receiver, float amount)
        {
            if (receiver.AccountNumber < 1) return false;
            if (!sender.TransferMoney(receiver, amount)) return false;
            await SaveBankAccountsAsync(new BankAccount[]{sender,receiver}).ConfigureAwait(false);
            await LogMoneyTransactionAsync(sender.Id, receiver.Id, TransactionType.BankTransfer, amount).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Saves transaction information to database
        /// </summary>
        /// <param name="source">Source of the transaction</param>
        /// <param name="receiver">Receiver of the transactions</param>
        /// <param name="type">Type of the transaction <see cref="TransactionType"/></param>
        /// <param name="amount">Amount of transaction</param>
        private void LogMoneyTransaction(int source, int receiver, TransactionType type, float amount)
        {
            _serverContext.MoneyTransactions.Add(new MoneyTransaction(source, receiver, type, amount));
            _serverContext.SaveChanges();
        }

        /// <summary>
        /// Saves transaction information to database
        /// </summary>
        /// <param name="source">Source of the transaction</param>
        /// <param name="receiver">Receiver of the transactions</param>
        /// <param name="type">Type of the transaction <see cref="TransactionType"/></param>
        /// <param name="amount">Amount of transaction</param>
        private async Task LogMoneyTransactionAsync(int source, int receiver, TransactionType type, float amount)
        {
            await _serverContext.MoneyTransactions.AddAsync(new MoneyTransaction(source, receiver, type, amount)).ConfigureAwait(false);
            await _serverContext.SaveChangesAsync();
        }

        private async Task SaveBankAccountAsync(BankAccount bankAccount)
        {
            _serverContext.Update(bankAccount);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task SaveBankAccountsAsync(BankAccount[] bankAccounts)
        {
            _serverContext.UpdateRange(bankAccounts);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private void SaveBankAccounts(BankAccount[] bankAccounts)
        {
            _serverContext.UpdateRange(bankAccounts);
            _serverContext.SaveChanges();
        }
    }
}