using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Modules.CharacterModule;
using AltVStrefaRPServer.Services;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Vehicle
{
    public class VehicleShopsHandler
    {
        private VehicleShopsManager _vehicleShopsManager;
        private INotificationService _notificationService;

        public VehicleShopsHandler(VehicleShopsManager vehicleShopsManager, INotificationService notificationService)
        {
            _vehicleShopsManager = vehicleShopsManager;
            _notificationService = notificationService;

            AltAsync.On<IPlayer, int>("OpenVehicleShop", async (player, shopId) => await OpenVehicleShopEvent(player, shopId));
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
    }
}
