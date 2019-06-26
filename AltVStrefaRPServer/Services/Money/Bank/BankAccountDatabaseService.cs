using System;
using System.Collections.Generic;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Money.Bank
{
    public class BankAccountDatabaseService : IBankAccountDatabaseService
    {
        private Func<ServerContext> _factory;

        public BankAccountDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<BankAccount> GetAllBankAccounts()
        {
            using (var context = _factory.Invoke())
            {
                return context.BankAccounts;
            }
        }
    }
}
