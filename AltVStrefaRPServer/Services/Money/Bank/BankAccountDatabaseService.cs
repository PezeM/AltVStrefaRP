using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task AddNewBankAccount(Character character)
        {
            using (var context = _factory.Invoke())
            {
                await context.BankAccounts.AddAsync(character.BankAccount);
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<MoneyTransaction>> GetTransferHistory(Character character, int numberOfLastTransactions = 50)
        {
            using (var context = _factory.Invoke())
            {
                return await context.MoneyTransactions.AsNoTracking()
                    .Where(t => t.Receiver == character.GetFullName() || t.Source == character.GetFullName())
                    .Take(numberOfLastTransactions).ToListAsync();
            }
        }
    }
}
