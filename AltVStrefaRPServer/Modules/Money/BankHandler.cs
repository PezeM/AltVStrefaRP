using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Client;
using AltVStrefaRPServer.Modules.Character;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Money;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Money
{
    public class BankHandler
    {
        private IMoneyService _moneyService;
        private ServerContext _serverContext;
        private INotificationService _notificationService;

        public BankHandler(IMoneyService moneyService, INotificationService notificationService, ServerContext serverContext)
        {
            _moneyService = moneyService;
            _serverContext = serverContext;
            _notificationService = notificationService;

            GetBankAccountsNumber(); // Temporary

            AltAsync.OnClient("tryToOpenBankMenu", TryToOpenBankMenu);
            AltAsync.OnClient("createBankAccount", CreateBankAccountAsync);
            AltAsync.OnClient("DepositMoneyToBank", DepositMoneyToBankAsync);
            AltAsync.OnClient("WithdrawMoneyFromBank", WithdrawMoneyFromBankAsync);
            AltAsync.OnClient("TransferMoneyFromBankToBank", TransferMoneyFromBankToBankAsync);
        }

        public async Task CreateBankAccountAsync(IPlayer player, object[] args)
        {
            var startTime = Time.GetTimestampMs();
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null) return;
            if (character.BankAccount != null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Masz już konto w banku.", 4000);
                return;
            }

            character.BankAccount = new BankAccount
            {
                Money = 0,
                AccountNumber = GenerateBankAccountNumber(),
            };

            _serverContext.Characters.Update(character);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);

            await _notificationService.ShowSuccessNotificationAsync(player,
                $"Otworzyłeś nowe konto w banku. Twój numer konta to: {character.BankAccount.AccountNumber}.", 7000);
            AltAsync.Log($"{character.Id} created new bank account ({character.BankAccount.AccountNumber}) in {Time.GetTimestampMs() - startTime}ms.");
        }

        public async Task TryToOpenBankMenu(IPlayer player, object[] args)
        {
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null) return;

            // Not needed because of relations
            //var bankAccount = await _serverContext.BankAccounts.AsNoTracking()
            //    .FirstOrDefaultAsync(b => b.CharacterId == character.Id).ConfigureAwait(false);

            if (character.BankAccount == null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Nie posiadsz konta w banku.", 4000).ConfigureAwait(false);
                return;
            }

            player.Emit("openBankMenu", JsonConvert.SerializeObject(
                new BankAccountInformationModel(character.GetFullName(), character.BankAccount.AccountNumber, character.BankAccount.Money)));
        }

        private async Task WithdrawMoneyFromBankAsync(IPlayer player, object[] args)
        {
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null || character.BankAccount == null) return;
            if (!int.TryParse(args[0].ToString(), out int moneyToWithdraw))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Wystąpił bład podczas wypłaty pieniędzy.").ConfigureAwait(false);
                return;
            }

            if (await _moneyService.WithdrawMoneyFromBankAccountAsync(character, character.BankAccount, moneyToWithdraw))
            {
                AltAsync.Log($"{character.Id} withdraw {moneyToWithdraw}$ from his bank account.");
                await _notificationService.ShowSuccessNotificationAsync(player, 
                    $"Pomyślnie wypłacono {moneyToWithdraw}$ z konta. Obecny stan konta wynosi {character.BankAccount.Money}$.").ConfigureAwait(false);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Nie udało się wypłacić pieniędzy z konta.").ConfigureAwait(false);
            }

        }

        private async Task DepositMoneyToBankAsync(IPlayer player, object[] args)
        {
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null || character.BankAccount == null) return;
            if (!int.TryParse(args[0].ToString(), out int moneyToDeposit))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Wystąpił bład podczas wpłaty pieniędzy.").ConfigureAwait(false);
                return;
            }

            if (await _moneyService.DepositMoneyToBankAccountAsync(character, character.BankAccount, moneyToDeposit))
            {
                AltAsync.Log($"{character.Id} deposited {moneyToDeposit}$ to his bank account.");
                await _notificationService.ShowSuccessNotificationAsync(player, 
                    $"Pomyślnie wpłacono {moneyToDeposit}$ na konto. Obecny stan konta wynosi {character.BankAccount.Money}$.").ConfigureAwait(false);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Nie udało się wpłacić pieniędzy na konto.").ConfigureAwait(false);
            }
        }

        private async Task TransferMoneyFromBankToBankAsync(IPlayer player, object[] args)
        {
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null || character.BankAccount == null) return;
            if (!int.TryParse(args[0].ToString(), out int moneyToTransfer) || !int.TryParse(args[1].ToString(), out int receiverAccountNumber))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Wystąpił bład podczas wykonywania przelewu.").ConfigureAwait(false);
                return;
            }

            // Temporary, preferable to load all accounts to memory on server start
            var receiverBankAccount = await _serverContext.BankAccounts.AsNoTracking()
                                        .FirstOrDefaultAsync(a => a.AccountNumber == receiverAccountNumber).ConfigureAwait(false);
            if (receiverBankAccount == null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Podano błędy numer konta bankowego.").ConfigureAwait(false);
                return;
            }

            if (await _moneyService.TransferMoneyFromBankAccountToBankAccountAsync(character.BankAccount, receiverBankAccount, moneyToTransfer))
            {
                AltAsync.Log($"{character.Id} transfered {moneyToTransfer} from his bank account to {receiverBankAccount} account.");
                await _notificationService.ShowSuccessNotificationAsync(player, 
                    $"Pomyślnie przesłano {moneyToTransfer}$ na konto o numerze{receiverBankAccount}. " +
                    $"Twój aktualny stan konta wynosi {character.BankAccount.Money}$.").ConfigureAwait(false);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Nie udało się przelać pieniędzy.").ConfigureAwait(false);
            }
        }

        #region Temporary account number creator

        private static Random _rng = new Random();
        private static readonly object _lock = new object();
        private List<int> _bankAccountsNumber;

        public void GetBankAccountsNumber()
        {
            Thread.SpinWait(10);
            var bankAccounts = _serverContext.BankAccounts.ToList();
            _bankAccountsNumber = bankAccounts.Select(b => b.AccountNumber).ToList();
        }

        /// <summary>
        /// Generates random non used bank account number(between 100000 and 999999)
        /// </summary>
        /// <returns></returns>
        public int GenerateBankAccountNumber()
        {
            lock (_lock)
            {
                int accountNumber = 0;
                do
                {
                    accountNumber = _rng.Next(100000, 1000000);
                } while (_bankAccountsNumber.Contains(accountNumber));
                _bankAccountsNumber.Add(accountNumber);
                return accountNumber;
            }
        }

        #endregion
    }
}
