using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Money.Bank
{
    public class BankAccountDatabaseService : IBankAccountDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public BankAccountDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<BankAccount> GetAllBankAccounts()
        {
            using (var context = _factory.Invoke())
            {
                return context.BankAccounts.ToList();
            }
        }

        public async Task AddNewBankAccountAsync(Character character)
        {
            using (var context = _factory.Invoke())
            {
                await context.BankAccounts.AddAsync(character.BankAccount);
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateBankAccountAsync(BankAccount bankAccount)
        {
            using (var context = _factory.Invoke())
            {
                context.BankAccounts.Update(bankAccount);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<MoneyTransaction>> GetTransferHistoryAsync(Character character, int numberOfLastTransactions = 50)
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
