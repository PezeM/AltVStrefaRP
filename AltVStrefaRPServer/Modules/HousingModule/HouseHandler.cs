using System;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory.Items.Keys;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Housing;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.HousingModule
{
    public class HouseHandler
    {
        private readonly INotificationService _notificationService;
        private readonly IHousesManager _housesManager;
        private readonly IBuyHouseService _buyHouseService;
        private readonly ILogger<HouseHandler> _logger;

        public HouseHandler(INotificationService notificationService, IHousesManager housesManager, IBuyHouseService buyHouseService, 
            ILogger<HouseHandler> logger)
        {
            _notificationService = notificationService;
            _housesManager = housesManager;
            _buyHouseService = buyHouseService;
            _logger = logger;
            Alt.OnColShape += AltOnOnColShape;
            
            Alt.On<IStrefaPlayer>("HouseEnterInteractionMenu", HouseEnterInteractionMenu);
            Alt.On<IStrefaPlayer>("TryEnterHouse", TryEnterHouse);
            Alt.On<IStrefaPlayer, int>("TryEnterHouse", TryEnterHotelRoom);
            Alt.On<IStrefaPlayer>("TryLeaveHouse", TryLeaveHouse);
            Alt.On<IStrefaPlayer>("TryOpenHouseDoor", TryOpenHouseDoor);
            Alt.On<IStrefaPlayer, int>("TryOpenHouseDoor", TryOpenHouseDoor);
            Alt.On<IStrefaPlayer>("TryCloseHouseDoor", TryCloseHouseDoor);
            Alt.On<IStrefaPlayer, int>("TryCloseHouseDoor", TryCloseHouseDoor);
            AltAsync.On<IStrefaPlayer, Task>("TryBuyHouse", TryBuyHouseAsync);
            AltAsync.On<IStrefaPlayer, int, Task>("TryRentHotelRoom", TryRentHotelRoom);
            AltAsync.On<IStrefaPlayer, int, int, Task>("TryToCreateNewHouse", TryToCreateNewHouseAsync);
        }

        private void AltOnOnColShape(IColShape colshape, IEntity entity, bool entered)
        {
            if (!(entity is IStrefaPlayer player)) return;
            if (!(colshape is IStrefaColshape strefaColshape)) return;
            if (strefaColshape.HouseId == 0) return;

            player.HouseEnterColshape = entered ? strefaColshape.HouseId : 0;
            player.Emit("inHouseEnterColshape", entered);
            _logger.LogDebug(
                entered
                    ? "Player entered house colshape with ID {houseColshapeId}"
                    : "Player left house colshape with ID {houseColshapeId}", strefaColshape.HouseId);
        }
        
        private void HouseEnterInteractionMenu(IStrefaPlayer player)
        {
            if (player.HouseEnterColshape == 0) return;
            if (!_housesManager.TryGetHouseBuilding(player.HouseEnterColshape, out var house)) return;

            player.Emit("showHouseEnterInteractionMenu", house);
        }
        
        private void TryEnterHouse(IStrefaPlayer player)
        {
            if(player.HouseEnterColshape == 0)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Brak wejścia do mieszkania w pobliżu", 3500);
                return;
            }

            if (!_housesManager.TryGetHouseBuilding(player.HouseEnterColshape, out var houseBuilding) || !(houseBuilding is House house))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie istnieje takie mieszkanie", 3500);
                return;
            }
            
            if (!house.Flat.MovePlayerInside(player))
            {
                _notificationService.ShowErrorNotification(player, "Zamknięte", "Mieszkanie jest zamknięte", 2500);
            }
        }

        private void TryEnterHotelRoom(IStrefaPlayer player, int hotelRoomNumber)
        {
            if(player.HouseEnterColshape == 0)
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Brak wejścia do hotelu w pobliżu", 3500);
                return;
            }

            if (!_housesManager.TryGetHouseBuilding(player.HouseEnterColshape, out var houseBuilding) || !(houseBuilding is Hotel hotel))
            {
                _notificationService.ShowErrorNotification(player, "Błąd", "Nie istnieje takie mieszkanie", 3500);
                return;
            }

            if (hotel.TryGetHotelRoom(hotelRoomNumber, out var hotelRoom))
            {
                _notificationService.ShowErrorNotification(player, "Brak pokoju", "Nie znaleziono takiego pokoju", 3000);
                return;
            }

            if (hotelRoom.MovePlayerInside(player))
            {
                _notificationService.ShowErrorNotification(player, "Zamknięte", "Mieszkanie jest zamknięte", 2500);
            }
        }
        
        private void TryLeaveHouse(IStrefaPlayer player)
        {
            player.EnteredFlat?.MovePlayerOutside(player);
        }

        private void TryCloseHouseDoor(IStrefaPlayer player)
        {
            if (!player.TryGetCharacter(out var character)) return;
            Flat flat;
            if (player.EnteredFlat != null) flat = player.EnteredFlat;
            else
            {
                if (!_housesManager.TryGetHouseBuilding(player.HouseEnterColshape, out var houseBuilding)) return;
                if (!(houseBuilding is House house)) return;
                flat = house.Flat;
            }

            if (flat.IsLocked) return;
            CloseHouseDoor(player, character, flat);
        }

        private void TryCloseHouseDoor(IStrefaPlayer player, int hotelRoomNumber)
        {
            if (!player.TryGetCharacter(out var character)) return;
            Flat flat;
            if (player.EnteredFlat != null) flat = player.EnteredFlat;
            else
            {
                if (_housesManager.TryGetHouseBuilding(player.HouseEnterColshape, out var houseBuilding)) return;
                if (!(houseBuilding is Hotel hotel)) return;
                if (hotel.TryGetHotelRoom(hotelRoomNumber, out var hotelRoom)) return;
                flat = hotelRoom;
            }

            if (flat.IsLocked) return;

            CloseHouseDoor(player, character, flat);
        }

        private void CloseHouseDoor(IPlayer player, Character character, Flat flat)
        {
            var keys = character.Inventory.GetItems<HouseKeyItem>();
            if (keys == null)
            {
                _notificationService.ShowErrorNotification(player, "Brak kluczy", "Nie posiadasz przy sobie żadnych kluczy",
                    3500);
                return;
            }

            var correctKeys = keys.FirstOrDefault(k => k.LockPattern == flat.LockPattern);
            if (correctKeys == null)
            {
                _notificationService.ShowErrorNotification(player, "Brak kluczy", "Nie posiadasz kluczy do tego mieszkania",
                    3500);
                return;
            }

            flat.IsLocked = true;
            player.Emit("successfullyToggledHouseLock", flat.IsLocked); // Play some sound and show notificati
        }

        private void TryOpenHouseDoor(IStrefaPlayer player)
        {
            if (!player.TryGetCharacter(out var character)) return;
            Flat flat;
            if (player.EnteredFlat != null) flat = player.EnteredFlat;
            else
            {
                if (!_housesManager.TryGetHouseBuilding(player.HouseEnterColshape, out var houseBuilding)) return;
                if (!(houseBuilding is House house)) return;
                flat = house.Flat;
            }

            if (!flat.IsLocked) return;

            OpenHouseDoor(player, character, flat);
        }

        private void TryOpenHouseDoor(IStrefaPlayer player, int hotelRoomNumber)
        {
            if (!player.TryGetCharacter(out var character)) return;
            Flat flat;
            if (player.EnteredFlat != null) flat = player.EnteredFlat;
            else
            {
                if (_housesManager.TryGetHouseBuilding(player.HouseEnterColshape, out var houseBuilding)) return;
                if (!(houseBuilding is Hotel hotel)) return;
                if (hotel.TryGetHotelRoom(hotelRoomNumber, out var hotelRoom)) return;
                flat = hotelRoom;
            }

            if (!flat.IsLocked) return;

            OpenHouseDoor(player, character, flat);
        }

        private void OpenHouseDoor(IPlayer player, Character character, Flat flat)
        {
            var keys = character.Inventory.GetItems<HouseKeyItem>();
            if (keys == null)
            {
                _notificationService.ShowErrorNotification(player, "Brak kluczy", "Nie posiadasz przy sobie żadnych kluczy",
                    3500);
                return;
            }

            var correctKeys = keys.FirstOrDefault(k => k.LockPattern == flat.LockPattern);
            if (correctKeys == null)
            {
                _notificationService.ShowErrorNotification(player, "Brak kluczy", "Nie posiadasz kluczy do tego mieszkania",
                    3500);
                return;
            }

            flat.IsLocked = false;
            player.Emit("successfullyToggledHouseLock", flat.IsLocked); // Play some sound and show notification
        }

        public async Task TryBuyHouseAsync(IStrefaPlayer player)
        {
            if (player.HouseEnterColshape == 0) return;
            if (!_housesManager.TryGetHouseBuilding(player.HouseEnterColshape, out var houseBuilding)) return;
            if (!(houseBuilding is House house)) return;
            if (!player.TryGetCharacter(out var character)) return;
            
            var response = await _buyHouseService.BuyHouseAsync(character, house);
            switch (response)
            {
                case BuyHouseResponse.HouseHasOwner:
                    _notificationService.ShowErrorNotificationLocked(player, "Mieszkanie zamieszkałe", "Mieszkanie posiada już właściciela", 3500);
                    break;
                case BuyHouseResponse.NotEnoughMoney:
                    _notificationService.ShowErrorNotificationLocked(player, "Brak pieniędzy", "Nie posiadasz odpowiedniej ilości pieniędzy aby zakupić te mieszkanie");
                    break;
                case BuyHouseResponse.NotEnoughSpaceInInventoryForKey:
                    _notificationService.ShowErrorNotificationLocked(player, "Brak miejsca", "Nie posiadasz wolnego miejsca w ekwipunku na klucze" );
                    break;
                case BuyHouseResponse.HouseBought:
                    _notificationService.ShowSuccessNotificationLocked(player, "Kupiono mieszkanie", $"Pomyślnie zakupiłeś mieszkanie za {house.Price}$", 3500);
                    break;
            }
        }

        private Task TryRentHotelRoom(IStrefaPlayer arg1, int arg2)
        {
            throw new NotImplementedException();
        }

        public async Task TryToCreateNewHouseAsync(IStrefaPlayer player, int price, int interiorId)
        {
            if (!player.TryGetCharacter(out var character)) return;
            if (character.Account.AdminLevel < AdminLevel.Admin)
            {
                _notificationService.ShowErrorNotificationLocked(player, "Brak uprawnień", "Nie posiadasz odpowiednich uprawnień do wykonania tej akcji");
                return;
            }

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
                        $"Stworzono mieszkanie z cena {price}$ i interiorem ID({interiorId})", 3500);
                    break;
            }
        }

        public async Task TryToCreateNewHotelAsync(IStrefaPlayer player, int price, int interiorId, int maxRooms)
        {
            if (!player.TryGetCharacter(out var character)) return;
            if (character.Account.AdminLevel < AdminLevel.Admin)
            {
                _notificationService.ShowErrorNotificationLocked(player, "Brak uprawnień", "Nie posiadasz odpowiednich uprawnień do wykonania tej akcji");
                return;
            }

            var result = await _housesManager.AddNewHotelAsync(new Position(player.Position.X, player.Position.Y, player.Position.Z - 1), price, maxRooms, interiorId);
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
                    _notificationService.ShowErrorNotificationLocked(player, "Stworzono hotel",
                        $"Stworzono hotel z cena {price}$, interiorem ID({interiorId}) i {maxRooms} pokojami", 3500);
                    break;
            }
        }
    }
}