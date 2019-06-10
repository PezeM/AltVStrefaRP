using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Services;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class TownHallFractionHandler
    {
        private FractionManager _fractionManager;
        private readonly INotificationService _notificationService;

        public TownHallFractionHandler(FractionManager fractionManager, INotificationService notificationService)
        {
            _fractionManager = fractionManager;
            _notificationService = notificationService;

            Alt.On<IPlayer, int, int, float>("TryToUpdateTax", TryToUpdateTax);    
        }

        private void TryToUpdateTax(IPlayer player, int fractionId, int taxId, float newTax)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (!_fractionManager.TryToGetFraction(fractionId, out Fraction fraction)) return;
            if (!(fraction is TownHallFraction townHallFraction)) return;

            if (!((fraction.GetEmployeeRank(character)?.IsHighestRank).Value))
            {
                _notificationService.ShowErrorNotfication(player, "Brak uprawnień",
                    "Nie posiadasz odpowiednich uprawnień do wykonania tej akcji.", 6500);
                return;
            }

            if (UpdateTax(taxId, newTax, townHallFraction))
            {
                _notificationService.ShowSuccessNotification(player, "Sukces", $"Pomyślnie ustawiono nowy podatek na {newTax * 100}%.", 6500);
            }
            else
            {
                _notificationService.ShowErrorNotfication(player, "Błąd", $"Nie udało się ustawić nowego podatku.");
            }
        }

        private static bool UpdateTax(int taxId, float newTax, TownHallFraction townHallFraction)
        {
            bool result = false;
            switch (taxId)
            {
                case 1:
                    result = townHallFraction.SetVehicleTax(newTax);
                    break;
                case 2:
                    result = townHallFraction.SetPropertyTax(newTax);
                    break;
                case 3:
                    result = townHallFraction.SetGunTaxk(newTax);
                    break;
                case 4:
                    result = townHallFraction.SetGlobalTax(newTax);
                    break;
            }

            return result;
        }
    }
}
