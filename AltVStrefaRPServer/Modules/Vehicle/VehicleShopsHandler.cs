using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Money;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleShopsHandler
    {
        private readonly VehicleShopsManager _vehicleShopsManager;
        private readonly INotificationService _notificationService;
        private readonly IMoneyService _moneyService;
        private readonly IVehiclesManager _vehiclesManager;

        public VehicleShopsHandler(VehicleShopsManager vehicleShopsManager, IVehiclesManager vehiclesManager,
            INotificationService notificationService, IMoneyService moneyService)
        {
            _vehicleShopsManager = vehicleShopsManager;
            _notificationService = notificationService;
            _moneyService = moneyService;
            _vehiclesManager = vehiclesManager;

            Alt.On<IPlayer, int>("OpenVehicleShop", OpenVehicleShopEvent);
            AltAsync.On<IPlayer, int, string>("BuyVehicle", async (player, shopId, vehicleModel) => await BuyVehicleEvent(player, shopId, vehicleModel));
        }

        private void OpenVehicleShopEvent(IPlayer player, int shopId)
        {
            var shop = _vehicleShopsManager.GetVehicleShop(shopId);
            if (shop == null)
            {
                _notificationService.ShowErrorNotification(player, "Błąd!", "Nie znaleziono takiego sklepu samochodowego.");
                return;
            }

            player.Emit("openVehicleShop", shop.VehicleShopId, JsonConvert.SerializeObject(shop.AvailableVehicles));
        }

        private async Task BuyVehicleEvent(IPlayer player, int shopId, string vehicleHash)
        {
            if (!player.TryGetCharacter(out Character character)) return;
            if (!long.TryParse(vehicleHash, out var vehicleModel))
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd z modelem", $"Wystąpił błąd z modelem pojazdu.");
            }

            var shop = _vehicleShopsManager.GetVehicleShop(shopId);
            if (shop == null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd!", "Nie znaleziono takiego sklepu samochodowego.");
                return;
            }

            var vehicleToBuy = shop.FindVehicleByModel(vehicleModel);
            if (vehicleToBuy == null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd!", "Nie znaleziono takiego pojazdu w tym sklepie samochodowym.");
                return;
            }

            if (!await _moneyService.TransferMoneyFromBankAccountToEntityAsync(character, shop, vehicleToBuy.Price, TransactionType.VehicleBuy))
            {
                await _notificationService.ShowErrorNotificationAsync(player, 
                    "Błąd!", $"Nie posiadasz {vehicleToBuy.Price}$ aby zakupić ten pojazd.", 6000);
                return;
            }

            // Player bought the vehicle, create vehicleModel and save it to database.
            await _vehiclesManager.CreateVehicleAsync(vehicleToBuy.VehicleModel.ToString(), shop.GetPositionOfBoughtVehicles(), 
                shop.GetRotationOfBoughtVehicles(), 0, character.Id, OwnerType.Character);
            await _notificationService.ShowSuccessNotificationAsync(player, "Zakupiono pojazd!", 
                $"Pomyślnie zakupiono pojazd {vehicleToBuy.VehicleModel.ToString()} za {vehicleToBuy.Price}$.");
        }
    }
}
