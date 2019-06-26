using System.Collections.Generic;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Money.Bank
{
    public interface IBankAccountDatabaseService
    {
        IEnumerable<BankAccount> GetAllBankAccounts();
    }
}
