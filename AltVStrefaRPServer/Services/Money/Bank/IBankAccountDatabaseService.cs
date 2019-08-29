using AltVStrefaRPServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Money.Bank
{
    public interface IBankAccountDatabaseService
    {
        IEnumerable<BankAccount> GetAllBankAccounts();
        Task AddNewBankAccountAsync(Character character);
        Task UpdateBankAccountAsync(BankAccount bankAccount);
        Task<List<MoneyTransaction>> GetTransferHistoryAsync(Character character, int numberOfLastTransactions = 50);
    }
}
