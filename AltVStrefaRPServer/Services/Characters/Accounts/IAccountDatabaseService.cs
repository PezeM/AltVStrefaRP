using AltVStrefaRPServer.Models;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Characters.Accounts
{
    public interface IAccountDatabaseService
    {
        Task<Account> GetAccountAsync(string username);
        Task<Account> GetAccountAsync(int accountId);
        Task<int> CheckIfAccountExistsAsync(string username);
        Task AddNewAccountAsync(Account account);
    }
}
