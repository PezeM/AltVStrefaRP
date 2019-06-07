using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Services;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class FractionHandler
    {
        private readonly FractionManager _fractionManager;
        private readonly INotificationService _notificationService;

        public FractionHandler(FractionManager fractionManager, INotificationService notificationService)
        {
            _fractionManager = fractionManager;
            _notificationService = notificationService;
        }

        public void OpenFractionMenu(Character character)
        {
            if (!_fractionManager.TryToGetFraction(character, out Fraction fraction))
            {
                _notificationService.ShowErrorNotfication(character.Player, "Brak frakcji", "Nie jesteś zatrudniony w żadnej frakcji.");
                return;
            }
        }
    }
}
