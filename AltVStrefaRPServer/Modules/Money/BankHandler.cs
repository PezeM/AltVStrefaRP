using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Client;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Money;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Money
{
    public class BankHandler
    {
        private IMoneyService _moneyService;
        private readonly ServerContext _serverContext;
        private INotificationService _notificationService;
        private BankAccountManager _bankAccountManager;

        public BankHandler(IMoneyService moneyService, INotificationService notificationService, ServerContext serverContext, 
            BankAccountManager bankAccountManager)
        {
            _moneyService = moneyService;
            _serverContext = serverContext;
            _notificationService = notificationService;
            _bankAccountManager = bankAccountManager;

            Alt.On<IPlayer>("TryToOpenBankMenu", TryToOpenBankMenu);
            AltAsync.On<IPlayer>("CreateBankAccount", async (player) => await CreateBankAccountAsync(player));
            AltAsync.On<IPlayer, int>("DepositMoneyToBank", async (player, money) => await DepositMoneyToBankAsync(player, money));
            AltAsync.On<IPlayer, int>("WithdrawMoneyFromBank", async (player, money) => await WithdrawMoneyFromBankAsync(player, money));
            AltAsync.On<IPlayer, int, int>("TransferMoneyFromBankToBank", async (player, money, receiver) 
                => await TransferMoneyFromBankToBankAsync(player, money, receiver));
            AltAsync.On<IPlayer>("GetTransferHistoryInfo", async (player) => await GetTransferHistoryInfoAsync(player));

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
                AltAsync.Log($"Error occured in adding new bank account. Account number: {character.BankAccount.AccountNumber}");
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Wystąpił błąd z tworzeniem nowego konta bankowego.", 5000);
                return;
            }

            await _serverContext.BankAccounts.AddAsync(character.BankAccount).ConfigureAwait(false);
            _serverContext.Characters.Update(character);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);

            await _notificationService.ShowSuccessNotificationAsync(player, "Nowe konto bankowe",
                $"Otworzyłeś nowe konto w banku. Twój numer konta to: {character.BankAccount.AccountNumber}.", 7000);
            AltAsync.Log($"{character.Id} created new bank account ({character.BankAccount.AccountNumber}) in {Time.GetTimestampMs() - startTime}ms.");
        }

        public void TryToOpenBankMenu(IPlayer player)
        {
            if (!player.TryGetCharacter(out Character character)) return;

            if (character.BankAccount == null)
            {
                _notificationService.ShowErrorNotfication(player, "Brak konta", "Nie posiadsz konta w banku.", 4000);
                return;
            }

            player.Emit("openBankMenu", JsonConvert.SerializeObject(
                new BankAccountInformationModel(character.GetFullName(), character.BankAccount.AccountNumber, character.BankAccount.Money)));
        }

        private async Task WithdrawMoneyFromBankAsync(IPlayer player, int money)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (character.BankAccount == null) return;

            if (await _moneyService.WithdrawMoneyFromBankAccountAsync(character, character.BankAccount, money).ConfigureAwait(false))
            {
                AltAsync.Log($"{character.Id} withdraw {money}$ from his bank account.");
                await player.EmitAsync("updateBankMoneyWithNotification",
                    $"Pomyślnie wypłacono {money}$ z konta. Obecny stan konta wynosi {character.BankAccount.Money}$.",
                    character.BankAccount.Money).ConfigureAwait(false);
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

            if (await _moneyService.DepositMoneyToBankAccountAsync(character, character.BankAccount, money).ConfigureAwait(false))
            {
                AltAsync.Log($"{character.Id} deposited {money}$ to his bank account.");
                await player.EmitAsync("updateBankMoneyWithNotification",
                    $"Pomyślnie wpłacono {money}$ na konto. Obecny stan konta wynosi {character.BankAccount.Money}$.",
                    character.BankAccount.Money);
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
            if (!_bankAccountManager.TryToGetBankAccountByNumber(receiver, out BankAccount receiverBankAccount))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Podano błędy numer konta bankowego.").ConfigureAwait(false);
                return;
            }


            if (await _moneyService.TransferMoneyFromBankAccountToBankAccountAsync(character.BankAccount, receiverBankAccount, money))
            {
                var receiverCharacter = CharacterManager.Instance.GetCharacterByBankAccount(receiverBankAccount.Id);

                // Receiver character is currently offline so don't send any notification to him
                if (receiverCharacter == null)
                {
                    AltAsync.Log($"{character.Id} transfered {money} from his bank account to {receiverBankAccount} account.");
                    await player.EmitAsync("updateBankMoneyWithNotification",
                        $"Pomyślnie przesłano {money}$ na konto o numerze {receiverBankAccount}. <br>" +
                        $"Twój aktualny stan konta wynosi {character.BankAccount.Money}$.",
                        character.BankAccount.Money).ConfigureAwait(false);
                }
                else
                {
                    AltAsync.Log($"{character.Id} transfered {money} from his bank account to {receiverBankAccount} account.");
                    await player.EmitAsync("updateBankMoneyWithNotification",
                        $"Pomyślnie przesłano {money}$ na konto o numerze {receiverBankAccount}. <br>" +
                        $"Twój aktualny stan konta wynosi {character.BankAccount.Money}$.",
                        character.BankAccount.Money).ConfigureAwait(false);
                    await _notificationService.ShowSuccessNotificationAsync(receiverCharacter.Player, "Otrzymano przelew!",
                        $"Właśnie otrzymałeś przelew od {character.GetFullName()} w wysokości {money}. <br>" +
                        $"Twój aktualny stan konta wynosi {receiverBankAccount.Money}$.",7000);
                }
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się przelać pieniędzy.").ConfigureAwait(false);
            }
        }

        private async Task GetTransferHistoryInfoAsync(IPlayer player)
        {
            if (!player.TryGetCharacter(out Character character)) return;

            var bankTransactionHistory = await _serverContext.MoneyTransactions.AsNoTracking()
                .Where(t => t.Receiver == character.GetFullName() || t.Source == character.GetFullName())
                .Take(50)
                .ToListAsync().ConfigureAwait(false);

            if (bankTransactionHistory.Count <= 0)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz jeszcze żadnych transakcji");
            }
            else
            {
                AltAsync.Log($"Bank transaction history for {character.Id} = {JsonConvert.SerializeObject(bankTransactionHistory)}");
                await player.EmitAsync("openTransactionHistory", JsonConvert.SerializeObject(bankTransactionHistory));
            }
        }
    }
}
