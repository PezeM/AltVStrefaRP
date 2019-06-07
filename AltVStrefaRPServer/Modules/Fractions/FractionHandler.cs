using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Services;
using Newtonsoft.Json;

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

            //CreateTestFraction();
        }

        public void OpenFractionMenu(Character character)
        {
            if (!_fractionManager.TryToGetFraction(character, out Fraction fraction))
            {
                _notificationService.ShowErrorNotfication(character.Player, "Brak frakcji", "Nie jesteś zatrudniony w żadnej frakcji.");
                return;
            }

            character.Player.Emit("openFractionMenu", JsonConvert.SerializeObject(fraction));
        }

        //public async Task CreateTestFraction()
        //{

        //}
    }
}
