using AltV.Net;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Services;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Businesses
{
    public class BusinessHandler
    {
        private BusinessManager _businessManager;
        private INotificationService _notificationService;

        public BusinessHandler(BusinessManager businessManager, INotificationService notificationService)
        {
            _businessManager = businessManager;
            _notificationService = notificationService;
            Alt.Log("Intialized business handler.");
        }

        public void OpenBusinessMenu(Character character)
        {
            if (character.Business == null)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Nie jesteś w żadnym biznesie.", 4000);
                return;
            }

            var businessRank = _businessManager.GetBusinessRankForPlayer(character);
            if (businessRank == null)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Nie masz ustalonych żadnych możliwości w biznesie.", 6000);
                return;
            }

            if (!businessRank.Permissions.CanOpenBusinessMenu)
            {
                _notificationService.ShowErrorNotfication(character.Player, "Nie możesz otwierać menu biznesu.",4000);
                return;
            }

            character.Player.Emit("openBusinessMenu", JsonConvert.SerializeObject(character.Business));
        }
    }
}
