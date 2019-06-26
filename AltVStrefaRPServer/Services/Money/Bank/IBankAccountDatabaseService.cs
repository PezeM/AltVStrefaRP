using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Money.Bank
{
    public interface IBankAccountDatabaseService
    {
        IEnumerable<BankAccount> GetAllBankAccounts();
        Task AddNewBankAccount(Character character);
        Task<List<MoneyTransaction>> GetTransferHistory(Character character, int numberOfLastTransactions = 50);
    }
}
