namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IBankAccountManager : IManager<BankAccount>
    {
        BankAccount GetBankAccountByCharacterId(int characterId);
        BankAccount GetBankAccountById(int bankAccountId);
        bool TryToGetBankAccountByNumber(int bankAccountNumber, out BankAccount bankAccount);
        bool AddNewBankAccount(BankAccount bankAccount);
        int GenerateBankAccountNumber();
    }
}