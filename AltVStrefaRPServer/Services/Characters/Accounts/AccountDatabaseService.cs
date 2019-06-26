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
        public Task<Account> GetAccountAsync(string username)
        {
            using (var context = _factory.Invoke())
            {
                return context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Username == username);
            }
        }

        /// <summary>
        /// Checks if account with given username is already in database
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns true if there is already account with given username</returns>
        public Task<int> CheckIfAccountExistsAsync(string username)
        {
            using (var context = _factory.Invoke())
            {
                return context.Accounts.Where(a => a.Username == username).CountAsync();
            }
        }

        /// <summary>
        /// Generates new hashed password and creates new account in the database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task CreateNewAccountAndSaveAsync(string username, string password)
        {
            using (var context = _factory.Invoke())
            {
                await context.Accounts.AddAsync(new Account
                {
                    Username = username,
                    Password = password,
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
