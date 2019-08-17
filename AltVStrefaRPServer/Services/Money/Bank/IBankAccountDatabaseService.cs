using AltVStrefaRPServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Money.Bank
{
    public interface IBankAccountDatabaseService
    {
        IEnumerable<BankAccount> GetAllBankAccounts();
        Task AddNewBankAccount(Character character);
        Task<List<MoneyTransaction>> GetTransferHistory(Character character, int numberOfLastTransactions = 50);
    }
}
