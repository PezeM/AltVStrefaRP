using AltV.Net;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Services;

namespace AltVStrefaRPServer.Modules.Admin
{
    public class AdminMenuHandler
    {
        private readonly INotificationService _notificationService;

        public AdminMenuHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
            Alt.On<IStrefaPlayer>("TryToOpenAdminMenu", TryOpenAdminMenu);    
        }

        private void TryOpenAdminMenu(IStrefaPlayer player)
        {
            if (!player.TryGetCharacter(out var character)) return;
            if(character.Account == null) return;
            if (character.Account.AdminLevel <= AdminLevel.TrialSupport)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie posiadasz odpowiednich uprawnień aby otworzyć menu admina", 3500);
                return;
            }

            player.Emit("openAdminMenu");
        }
    }
}
