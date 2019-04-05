using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Database;
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

            AltAsync.OnClient("tryToOpenBankMenu", TryToOpenBankMenu);
        }

        private async Task TryToOpenBankMenu(IPlayer player, object[] args)
        {
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null) return;

            var bankAccount = await _serverContext.BankAccounts.AsNoTracking()
                .FirstOrDefaultAsync(b => b.CharacterId == character.Id).ConfigureAwait(false);

            if (bankAccount == null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Nie posiadsz konta w banku", 5000).ConfigureAwait(false);
                return;
            }

            player.Emit("openBankMenu", JsonConvert.SerializeObject(
                new BankAccountInformationModel(character.GetFullName(), bankAccount.AccountNumber, bankAccount.Money)));
        }
    }
}
