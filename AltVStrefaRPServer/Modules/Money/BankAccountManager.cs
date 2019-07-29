using System;
using System.Collections.Concurrent;
using System.Linq;
using AltV.Net;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services.Money.Bank;

namespace AltVStrefaRPServer.Modules.Money
{
    public class BankAccountManager : IBankAccountManager
    {
        private ConcurrentDictionary<int, BankAccount> _bankAccounts;
        private readonly IBankAccountDatabaseService _bankAccountDatabaseService;
        private static readonly Random _rng = new Random();

        public BankAccountManager(IBankAccountDatabaseService bankAccountDatabaseService)
        {
            _bankAccountDatabaseService = bankAccountDatabaseService;
            _bankAccounts = new ConcurrentDictionary<int, BankAccount>();

            LoadBankAccountsFromDatabase();
        }

        public BankAccount GetBankAccountById(int bankAccountId)
            => _bankAccounts.Values.FirstOrDefault(b => b.Id == bankAccountId);

        public bool TryToGetBankAccountByNumber(int bankAccountNumber, out BankAccount bankAccount)
            => _bankAccounts.TryGetValue(bankAccountNumber, out bankAccount);

        public BankAccount GetBankAccountByCharacterId(int characterId)
            => _bankAccounts.Values.FirstOrDefault(b => b.CharacterId == characterId);

        public bool AddNewBankAccount(BankAccount bankAccount)
            => _bankAccounts.TryAdd(bankAccount.AccountNumber, bankAccount);

        /// <summary>
        /// Generates unique bank account number
        /// </summary>
        /// <returns>Unique bank account number</returns>
        public int GenerateBankAccountNumber()
        {
            int accountNumber;
            lock (_bankAccounts)
            {
                do
                {
                    accountNumber = _rng.Next(100000, 1000000);
                } while (_bankAccounts.ContainsKey(accountNumber));
                return accountNumber;
            }
        }

        private void LoadBankAccountsFromDatabase()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var bankAccount in _bankAccountDatabaseService.GetAllBankAccounts())
            {
                _bankAccounts.TryAdd(bankAccount.AccountNumber, bankAccount);
            }
            Alt.Log($"Loaded {_bankAccounts.Count} bank accounts from databse in {Time.GetTimestampMs() - startTime}ms.");
        }
    }
}
