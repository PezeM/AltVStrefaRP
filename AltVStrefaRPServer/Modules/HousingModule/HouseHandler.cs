using System;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Money;

namespace AltVStrefaRPServer.Modules.HousingModule
{
    public class HouseHandler
    {
        private readonly INotificationService _notificationService;
        private readonly IHousesManager _housesManager;
        private readonly IMoneyService _moneyService;

        public HouseHandler(INotificationService notificationService, IHousesManager housesManager, IMoneyService moneyService)
        {
            _notificationService = notificationService;
            _housesManager = housesManager;
            _moneyService = moneyService;
            Alt.OnColShape += AltOnOnColShape;
            
            Alt.On<IStrefaPlayer>("HouseInteractionMenu", HouseInteractionMenu);
            Alt.On<IStrefaPlayer>("TryEnterHouse", TryEnterHouse);
            Alt.On<IStrefaPlayer>("TryLeaveHouse", TryLeaveHouse);
            Alt.On<IStrefaPlayer>("TryBuyHouse", TryBuyHouse);
            AltAsync.On<IStrefaPlayer, int, int, Task>("TryToCreateNewHouse", TryToCreateNewHouseAsync);
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
        
        private void TryBuyHouse(IStrefaPlayer player)
        {
            if (player.HouseEnterColshape == 0) return;
            if (!_housesManager.TryGetHouse(player.HouseEnterColshape, out var house)) return;

            if (!house.HasOwner())
            {
                _notificationService.ShowErrorNotification(player, "Budynek zamieszkały", "Mieszkanie posiada już właściciela", 3500);
                return;
            }
        }
        
        public async Task TryToCreateNewHouseAsync(IStrefaPlayer player, int price, int interiorId)
        {
            if (!player.TryGetCharacter(out var character)) return;
            if (character.Account.AdminLevel < AdminLevel.Admin)
            {
                _notificationService.ShowErrorNotificationLocked(player, "Brak uprawnień", "Nie posiadasz odpowiednich uprawnień do wykonania tej akcji");
                return;
            };

            var result = await _housesManager.AddNewHouseAsync(player.Position, price, interiorId);
            switch (result)
            {
                case AddNewHouseResponse.WrongInteriorId:
                    _notificationService.ShowErrorNotificationLocked(player, "Błąd", "Podano złe id interioru", 3500);
                    break;
                case AddNewHouseResponse.InteriorNotFound:
                    _notificationService.ShowErrorNotificationLocked(player, "Błąd",
                        "Nie znaleziono interioru z podanym id", 3500);
                    break;
                case AddNewHouseResponse.CantCreateHouse:
                    _notificationService.ShowErrorNotificationLocked(player, "Błąd",
                        "Nie udało się tworzenie nowego mieszkania", 3500);
                    break;
                case AddNewHouseResponse.HouseCreated:
                    _notificationService.ShowErrorNotificationLocked(player, "Stworzono mieszkanie",
                        $"Stworzono mieszkanie z cena {price} i interiorem ID({interiorId})", 3500);
                    break;
            }
        }
    }
}