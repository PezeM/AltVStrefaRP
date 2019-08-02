using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Money;
using AltVStrefaRPServer.Services.Money.Bank;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Money
{
    public class BankHandler
    {
        private readonly IMoneyService _moneyService;
        private readonly IBankAccountDatabaseService _bankAccountDatabaseService;
        private readonly INotificationService _notificationService;
        private readonly IBankAccountManager _bankAccountManager;
        private readonly ILogger<BankHandler> _logger;

        public BankHandler(IMoneyService moneyService, INotificationService notificationService, IBankAccountDatabaseService banklAccountDatabaseService, 
            IBankAccountManager bankAccountManager, ILogger<BankHandler> logger)
        {
            _moneyService = moneyService;
            _bankAccountDatabaseService = banklAccountDatabaseService;
            _notificationService = notificationService;
            _bankAccountManager = bankAccountManager;
            _logger = logger;

            Alt.On<IPlayer>("TryToOpenBankMenu", TryToOpenBankMenu);
            AltAsync.On<IPlayer, Task>("CreateBankAccount", CreateBankAccountAsync);
            AltAsync.On<IPlayer, int, Task>("DepositMoneyToBank", DepositMoneyToBankAsync);
            AltAsync.On<IPlayer, int, Task>("WithdrawMoneyFromBank", WithdrawMoneyFromBankAsync);
            AltAsync.On<IPlayer, int, int, Task>("TransferMoneyFromBankToBank", TransferMoneyFromBankToBankAsync);
            AltAsync.On<IPlayer, Task>("GetTransferHistoryInfo", GetTransferHistoryInfoAsync);
            //CreateAtmBlips();
        }

        private void CreateAtmBlips()
        {
            foreach (var atm in Data.GtaLocations.Atms)
            {
                var blip = Alt.CreateBlip(BlipType.Pickup, atm.Value);
                blip.Color = 25;
                blip.Sprite = 108;
                Alt.Log($"Blip position: {blip.Position} exists: {blip.Exists} type: {blip.BlipType}");
            }
        }

        public async Task CreateBankAccountAsync(IPlayer player)
        {
            var startTime = Time.GetTimestampMs();
            if (!player.TryGetCharacter(out Character character)) return;
            if (character.BankAccount != null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Masz już konto w banku.", 4000);
                return;
            }

            character.BankAccount = new BankAccount
            {
                Money = 0,
                AccountNumber = _bankAccountManager.GenerateBankAccountNumber(),
            };

            if (!_bankAccountManager.AddNewBankAccount(character.BankAccount))
            {
                _logger.LogWarning("Errorn occured in creating new bank account. Account number {accountNumber}", character.BankAccount.AccountNumber);
                character.BankAccount = null;
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Wystąpił błąd z tworzeniem nowego konta bankowego.", 5000);
                return;
            }

            await _bankAccountDatabaseService.AddNewBankAccount(character);
            await _notificationService.ShowSuccessNotificationAsync(player, "Nowe konto bankowe",
                $"Otworzyłeś nowe konto w banku. Twój numer konta to: {character.BankAccount.AccountNumber}.", 7000);
            _logger.LogInformation("Character {characterName} CID({characterId}) created new bank account {bankAccountNumber} in {elapsedTime}ms", 
                character.GetFullName(), character.Id, character.BankAccount.AccountNumber, Time.GetElapsedTime(startTime));
        }

        public void TryToOpenBankMenu(IPlayer player)
        {
            if (!player.TryGetCharacter(out Character character)) return;

            if (character.BankAccount == null)
            {
                _notificationService.ShowErrorNotification(player, "Brak konta", "Nie posiadsz konta w banku.", 4000);
                return;
            }

            player.Emit("openBankMenu", JsonConvert.SerializeObject(
                new BankAccountInformationModel(character.GetFullName(), character.BankAccount.AccountNumber, character.BankAccount.Money)));
        }

        private async Task WithdrawMoneyFromBankAsync(IPlayer player, int money)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (character.BankAccount == null) return;

            if(await _moneyService.TransferMoneyFromEntityToEntityAsync(character.BankAccount, character, money, TransactionType.BankWithdraw))
            {
                await player.EmitAsync("updateBankMoneyWithNotification",
                    $"Pomyślnie wypłacono {money}$ z konta. Obecny stan konta wynosi {character.BankAccount.Money}$.",
                    character.BankAccount.Money).ConfigureAwait(false);
                _logger.LogInformation("Character {characterName} CID({characterId}) withdraw {money}$ to his bank account", character.GetFullName(), character.Id, money);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd!", "Nie udało się wypłacić pieniędzy z konta.").ConfigureAwait(false);
            }
        }

        private async Task DepositMoneyToBankAsync(IPlayer player, int money)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (character.BankAccount == null) return;

            if (await _moneyService.TransferMoneyFromEntityToEntityAsync(character, character.BankAccount, money, TransactionType.BankDeposit))
            {
                await player.EmitAsync("updateBankMoneyWithNotification",
                    $"Pomyślnie wpłacono {money}$ na konto. Obecny stan konta wynosi {character.BankAccount.Money}$.",
                    character.BankAccount.Money).ConfigureAwait(false);
                _logger.LogInformation("Character {characterName} CID({characterId}) deposited {money}$ to his bank account", character.GetFullName(), character.Id, money);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się wpłacić pieniędzy na konto.").ConfigureAwait(false);
            }
        }

        private async Task TransferMoneyFromBankToBankAsync(IPlayer player, int money, int receiver)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (character.BankAccount == null) return;
            if (!_bankAccountManager.TryGetBankAccountByNumber(receiver, out BankAccount receiverBankAccount))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Podano błędy numer konta bankowego.").ConfigureAwait(false);
                return;
            }

            if (await _moneyService.TransferMoneyFromEntityToEntityAsync(character.BankAccount, receiverBankAccount, money, TransactionType.BankTransfer))
            {
                receiverBankAccount.NotifyOnMoneyTransfer(character, money, _notificationService);
                await player.EmitAsync("updateBankMoneyWithNotification",
                    $"Pomyślnie przesłano {money}$ na konto o numerze {receiverBankAccount}. <br>" +
                    $"Twój aktualny stan konta wynosi {character.BankAccount.Money}$.",
                    character.BankAccount.Money).ConfigureAwait(false);
                _logger.LogInformation("Character {characterName} CID({characterId}) transfered {money}$ to bank account {@bankAccount}", 
                    character.GetFullName(), character.Id, money, receiverBankAccount);
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się przelać pieniędzy.").ConfigureAwait(false);
            }
        }

        private async Task GetTransferHistoryInfoAsync(IPlayer player)
        {
            if (!player.TryGetCharacter(out Character character)) return;

            var bankTransactionHistory = await _bankAccountDatabaseService.GetTransferHistory(character);
            if (bankTransactionHistory.Count <= 0)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz jeszcze żadnych transakcji");
            }
            else
            {
                await player.EmitAsync("openTransactionHistory", JsonConvert.SerializeObject(bankTransactionHistory));
            }
        }
    }
}
