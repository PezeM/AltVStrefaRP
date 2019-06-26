using System.Threading.Tasks;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Characters.Accounts
{
    public interface IAccountDatabaseService
    {
        Task<Account> GetAccountAsync(string username);
        Task CreateNewAccountAndSaveAsync(string username, string password);
        Task<int> CheckIfAccountExistsAsync(string username);
    }
}
