using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Modules.CharacterModule;
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
        private readonly VehicleManager _vehicleManager;

        public VehicleShopsHandler(VehicleShopsManager vehicleShopsManager, VehicleManager vehicleManager,
            INotificationService notificationService, IMoneyService moneyService)
        {
            _vehicleShopsManager = vehicleShopsManager;
            _notificationService = notificationService;
            _moneyService = moneyService;
            _vehicleManager = vehicleManager;

            AltAsync.On<IPlayer, int>("OpenVehicleShop", async (player, shopId) => await OpenVehicleShopEvent(player, shopId));
            AltAsync.On<IPlayer, int, long>("BuyVehicle", async (player, shopId, vehicleModel) => await BuyVehicleEvent(player, shopId, vehicleModel));
        }

        private async Task OpenVehicleShopEvent(IPlayer player, int shopId)
        {
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null) return;

            var shop = _vehicleShopsManager.GetVehicleShop(shopId);
            if (shop == null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd!", "Nie znaleziono takiego sklepu samochodowego.");
                return;
            }

            player.Emit("openVehicleShop", shop.VehicleShopId, JsonConvert.SerializeObject(shop.AvailableVehicles));
        }

        private async Task BuyVehicleEvent(IPlayer player, int shopId, long vehicleModel)
        {
            var character = CharacterManager.Instance.GetCharacter(player);
            if (character == null) return;

            var shop = _vehicleShopsManager.GetVehicleShop(shopId);
            if (shop == null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd!", "Nie znaleziono takiego sklepu samochodowego.");
                return;
            }

            var vehicleToBuy = shop.AvailableVehicles.FirstOrDefault(v => (long)v.VehicleModel == vehicleModel);
            if (vehicleToBuy == null)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd!", "Nie znaleziono takiego pojazdu w tym sklepie samochodowym.");
                return;
            }

            if (!_moneyService.RemoveMoney(character, vehicleToBuy.Price))
            {
                await _notificationService.ShowErrorNotificationAsync(player, 
                    "Błąd!", $"Nie posiadasz {vehicleToBuy.Price}$ aby zakupić ten pojazd.", 6000);
                return;
            }

            // Player bought the vehicle, create vehicleModel and save it to database.
            await _vehicleManager.CreateVehicleAsync(vehicleToBuy.VehicleModel.ToString(), shop.PositionOfBoughtVehicles, shop.RotationOfBoughtVehicles, 
                0, character.Id, OwnerType.Character);
            await _notificationService.ShowSuccessNotificationAsync(player, "Zakupiono pojazd!", 
                $"Pomyślnie zakupiono pojazd ${vehicleToBuy.VehicleModel.ToString()} za {vehicleToBuy.Price}$");
        }
    }
}
