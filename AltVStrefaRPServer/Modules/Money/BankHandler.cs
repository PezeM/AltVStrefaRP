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
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Money
{
    public class BankHandler
    {
        private IMoneyService _moneyService;
        private ServerContext _serverContext;
        private INotificationService _notificationService;
        private BankAccountManager _bankAccountManager;

        public BankHandler(IMoneyService moneyService, INotificationService notificationService, ServerContext serverContext, 
            BankAccountManager bankAccountManager)
        {
            _moneyService = moneyService;
            _serverContext = serverContext;
            _notificationService = notificationService;
            _bankAccountManager = bankAccountManager;

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
                AccountNumber = _bankAccountManager.GenerateBankAccountNumber(),
            };

            if (!_bankAccountManager.AddNewBankAccount(character.BankAccount))
            {
                AltAsync.Log($"Error occured in adding new bank account. Account number: {character.BankAccount.AccountNumber}");
                await _notificationService.ShowErrorNotificationAsync(player, "Wystąpił błąd z tworzeniem nowego konta bankowego.", 5000);
                return;
            }

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
                await player.EmitAsync("updateBankMoneyWithNotification",
                    $"Pomyślnie wypłacono {moneyToWithdraw}$ z konta. Obecny stan konta wynosi {character.BankAccount.Money}$.",
                    character.BankAccount.Money).ConfigureAwait(false);
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
                await player.EmitAsync("updateBankMoneyWithNotification",
                    $"Pomyślnie wpłacono {moneyToDeposit}$ na konto. Obecny stan konta wynosi {character.BankAccount.Money}$.",
                    character.BankAccount.Money).ConfigureAwait(false);
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

            var receiverBankAccount = _bankAccountManager.GetBankAccountByNumber(receiverAccountNumber);

            if (receiverBankAccount == null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Podano błędy numer konta bankowego.").ConfigureAwait(false);
                return;
            }

            if (await _moneyService.TransferMoneyFromBankAccountToBankAccountAsync(character.BankAccount, receiverBankAccount, moneyToTransfer))
            {
                var receiverCharacter = CharacterManager.Instance.GetCharacterByBankAccount(receiverBankAccount.Id);

                // Receiver character is currently offline so don't send any notification to him
                if (receiverCharacter == null)
                {
                    AltAsync.Log($"{character.Id} transfered {moneyToTransfer} from his bank account to {receiverBankAccount} account.");
                    await player.EmitAsync("updateBankMoneyWithNotification",
                        $"Pomyślnie przesłano {moneyToTransfer}$ na konto o numerze {receiverBankAccount}. <br>" +
                        $"Twój aktualny stan konta wynosi {character.BankAccount.Money}$.",
                        character.BankAccount.Money).ConfigureAwait(false);
                }
                else
                {
                    AltAsync.Log($"{character.Id} transfered {moneyToTransfer} from his bank account to {receiverBankAccount} account.");
                    await player.EmitAsync("updateBankMoneyWithNotification",
                        $"Pomyślnie przesłano {moneyToTransfer}$ na konto o numerze {receiverBankAccount}. <br>" +
                        $"Twój aktualny stan konta wynosi {character.BankAccount.Money}$.",
                        character.BankAccount.Money).ConfigureAwait(false);
                    await _notificationService.ShowSuccessNotificationAsync(receiverCharacter.Player,
                        $"Właśnie otrzymałeś przelew od {character.GetFullName()} w wysokości {moneyToTransfer}. <br>" +
                        $"Twój aktualny stan konta wynosi {receiverBankAccount.Money}$.",7000);
                }
            }
            else
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Nie udało się przelać pieniędzy.").ConfigureAwait(false);
            }
        }
    }
}
