using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services;

namespace AltVStrefaRPServer.Modules.HousingModule
{
    public class HouseHandler
    {
        private readonly INotificationService _notificationService;
        private IHousesManager _housesManager;

        public HouseHandler(INotificationService notificationService, IHousesManager housesManager)
        {
            _notificationService = notificationService;
            _housesManager = housesManager;
            Alt.OnColShape += AltOnOnColShape;
            
            Alt.On<IStrefaPlayer>("HouseInteractionMenu", HouseInteractionMenu);
            Alt.On<IStrefaPlayer>("TryEnterHouse", TryEnterHouse);
            Alt.On<IStrefaPlayer>("TryLeaveHouse", TryLeaveHouse);
        }
      
        private void AltOnOnColShape(IColShape colshape, IEntity entity, bool entered)
        {
            if (!(entity is IStrefaPlayer player)) return;
            if (!(colshape is IStrefaColshape strefaColshape)) return;
            if (strefaColshape.HouseId == 0) return;

            player.HouseEnterColshape = entered ? strefaColshape.HouseId : 0;
            player.Emit("inHouseEnterColshape", entered);
        }
        
        private void HouseInteractionMenu(IStrefaPlayer player)
        {
            if (player.HouseEnterColshape == 0) return;
            if (!_housesManager.TryGetHouse(player.HouseEnterColshape, out var house)) return;

            player.Emit("showHouseInteractionMenu", house);
        }
        
        private void TryEnterHouse(IStrefaPlayer player)
        {
            if(player.HouseEnterColshape == 0)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Brak wejścia do mieszkania w pobliżu", 3500);
                return;
            }

            if (!_housesManager.TryGetHouse(player.HouseEnterColshape, out var house))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie istnieje takie mieszkanie", 3500);
                return;
            }

            if (house.IsLocked)
            {
                _notificationService.ShowErrorNotification(player, "Zamknięte", "Mieszkanie jest zamknięte", 2500);
                return;
            }

            house.MovePlayerInside(player);
        }

        private void TryLeaveHouse(IStrefaPlayer player)
        {
            // Player is not inside house
            if (player.HouseId == 0) return;
            if (!_housesManager.TryGetHouse(player.HouseId, out var house)) return;
            
            house.MovePlayerOutside(player);
        }
    }
}