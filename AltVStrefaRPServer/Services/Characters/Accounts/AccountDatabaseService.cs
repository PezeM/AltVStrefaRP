using System;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Characters.Accounts
{
    public class AccountDatabaseService : IAccountDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public AccountDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Gets account from database by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Account if found,null if account is not found</returns>
        public async Task<Account> GetAccountAsync(string username)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Accounts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Username == username);
            }
        }

        public async Task<Account> GetAccountAsync(int accountId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Accounts
                    .FindAsync(accountId);
            }
        }

        /// <summary>
        /// Checks if account with given username is already in database
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns true if there is already account with given username</returns>
        public async Task<int> CheckIfAccountExistsAsync(string username)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Accounts
                    .Where(a => a.Username == username)
                    .CountAsync();
            }
        }

        /// <summary>
        /// Adds new account to database
        /// </summary>
        /// <param name="account">The account to add</param>
        /// <returns></returns>
        public async Task AddNewAccountAsync(Account account)
        {
            using (var context = _factory.Invoke())
            {
                context.Accounts.Add(account);
                await context.SaveChangesAsync();
            }
        }
    }
}
