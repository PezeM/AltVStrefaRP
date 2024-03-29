﻿using AltV.Net.Elements.Entities;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IBankAccountManager : IManager<BankAccount>
    {
        BankAccount GetBankAccountByCharacterId(int characterId);
        BankAccount GetBankAccountById(int bankAccountId);
        bool TryGetBankAccount(IPlayer player, out BankAccount bankAccount);
        bool TryGetBankAccountByNumber(int bankAccountNumber, out BankAccount bankAccount);
        bool AddNewBankAccount(BankAccount bankAccount);
        int GenerateBankAccountNumber();
    }
}