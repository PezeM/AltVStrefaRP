using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Models.Vehicles;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Inventories;
using Microsoft.Extensions.Logging;
using Position = AltV.Net.Data.Position;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoryHandler
    {
        private readonly IInventoriesManager _inventoriesManager;
        private readonly IInventoryDatabaseService _inventoryDatabaseService;
        private readonly INotificationService _notificationService;
        private readonly IVehiclesManager _vehiclesManager;
        private readonly IInventoryTransferService _inventoryTransferService;
        private readonly IInventoryEquipService _inventoryEquipService;
        private readonly ILogger<InventoryHandler> _logger;

        public InventoryHandler(IInventoriesManager inventoriesManager, IInventoryDatabaseService inventoryDatabaseService, INotificationService notificationService, 
            IVehiclesManager vehiclesManager, IInventoryTransferService inventoryTransferService, IInventoryEquipService inventoryEquipService,
            ILogger<InventoryHandler> logger)
        {
            _inventoriesManager = inventoriesManager;
            _inventoryDatabaseService = inventoryDatabaseService;
            _notificationService = notificationService;
            _vehiclesManager = vehiclesManager;
            _inventoryTransferService = inventoryTransferService;
            _inventoryEquipService = inventoryEquipService;
            _logger = logger;

            Alt.On<IStrefaPlayer, IMyVehicle, bool>("OpenVehicleInventory", OpenVehicleInventory);
            Alt.On<IPlayer>("GetPlayerInventory", GetPlayerInventory);
            AltAsync.On<IPlayer, int, int, Position, Task>("DropItem", DropItemAsync);
            AltAsync.On<IPlayer, int, Task>("UseInventoryItem", UseInventoryItemAsync);
            AltAsync.On<IStrefaPlayer, int, int, int, Task>("InventoryDropItem", InventoryDropItemAsync);
            AltAsync.On<IPlayer, int, int, Task>("InventoryRemoveItem", InventoryRemoveItemAsync);
            AltAsync.On<IPlayer, int, int, Task>("PickupDroppedItem", PickupDroppedItemAsync);
            Alt.On<IStrefaPlayer, int, int, int>("InventoryMoveItem", InventoryMoveItem);
            AltAsync.On<IStrefaPlayer, int, int, int, int, Task>("InventoryTryTransferItem", InventoryTryTransferItemAsync);
            Alt.On<IStrefaPlayer, int, int, int>("InventoryTryStackItem", InventoryTryStackItem);
            AltAsync.On<IStrefaPlayer, int, int, int, int, Task>("InventoryTryStackItemBetweenInventories", InventoryTryStackItemBetweenInventoriesAsync);
            AltAsync.On<IStrefaPlayer, int, int, int, Task>("InventoryTryEquipItem", InventoryTryEquipItemAsync);
            AltAsync.On<IStrefaPlayer, int, int, int, int, Task>("InventoryTryUnequipItem", InventoryTryUnequipItemAsync);
        }

        private async Task InventoryTryTransferItemAsync(IStrefaPlayer player, int sourceInventoryId, int receiverInventoryId, int itemId, int newSlot)
        {
            throw new System.NotImplementedException();
        }

        private void InventoryMoveItem(IStrefaPlayer player, int inventoryId, int itemId, int newSlot)
        {
            if(!player.TryGetCharacter(out var character)) return;
            var inventory = InventoriesHelper.GetCorrectInventory(player, character, inventoryId);

            var response = InventoryMoveItemResponse.InventoryNotFound;
            if(inventory != null)
            {
                response = inventory.MoveItemToSlot(itemId, newSlot);
            }

            switch (response)
            {
                case InventoryMoveItemResponse.InventoryNotFound:
                case InventoryMoveItemResponse.ItemNotFound:
                    player.Emit("inventoryMoveItemResponse", false);
                    break;
                case InventoryMoveItemResponse.SlotOccupied:
                    _notificationService.ShowErrorNotification(player, "Błąd", "Jakiś przedmiot znajduję się już w tym miejscu");
                    player.Emit("inventoryMoveItemResponse", false);
                    break;
                case InventoryMoveItemResponse.ItemMoved:
                    player.Emit("inventoryMoveItemResponse", true);
                    break;
            }
        }

        private void GetPlayerInventory(IPlayer player)
        {
            var startTime = Time.GetTimestampMs();
            if(!player.TryGetCharacter(out var character)) return;
            player.Emit("populatePlayerInventory", InventoryContainerConverter.ConvertFromCharacterInventory(character), 
                InventoryContainerConverter.ConvertFromEquippedInventory(character));
            _logger.LogDebug("Send player inventory in {elapsedTime}ms", Time.GetElapsedTime(startTime));
        }

        private void OpenVehicleInventory(IStrefaPlayer player, IMyVehicle vehicle, bool getOwnInventory)
        {
            if (!_vehiclesManager.TryGetVehicleModel(vehicle, out var vehicleModel)) return;

            var addonationalInventoryContainer = InventoryContainerConverter.ConvertFromVehicleInventory(vehicleModel);
            player.LastOpenedInventory = vehicleModel.Inventory;

            if (!getOwnInventory)
            {
                player.Emit("populateAddonationalInventoryContainer", addonationalInventoryContainer);
                _logger.LogDebug("Emited only vehicle inventory");
                return;
            }

            if (!player.TryGetCharacter(out var character)) return;
            var inventoryItems = InventoryContainerConverter.ConvertFromCharacterInventory(character);
            var equippedItems = InventoryContainerConverter.ConvertFromEquippedInventory(character);

            player.Emit("populateAddonationalInventoryContainer", addonationalInventoryContainer, inventoryItems, equippedItems);
            _logger.LogDebug("Emited vehicle inventory with player inventory");
        }

        public async Task UseInventoryItemAsync(IPlayer player, int itemId)
        {
            if (!player.TryGetCharacter(out var character)) return;
            var response = await character.Inventory.UseItemAsync(character, itemId, _inventoryDatabaseService);
            switch (response)
            {
                case InventoryUseResponse.ItemNotFound:
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz takiego przedmiotu.");
                    break;
                case InventoryUseResponse.ItemNotUsed:
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się użyć tego przedmiotu.");
                    break;
                case InventoryUseResponse.ItemUsed:
                    await _notificationService.ShowSuccessNotificationAsync(player, "Błąd", "Pomyślnie użyto przedmiot.");
                    //TODO: If he used the item, remove quantity, if the item was removed, remove it from user inventory UI
                    break;
            }
        }

        public async Task DropItemAsync(IPlayer player, int itemId, int amount, Position position)
        {
            if (!player.TryGetCharacter(out var character)) return;
            var response = await character.Inventory.DropItemAsync(itemId, amount, position, _inventoriesManager, _inventoryDatabaseService)
                .ConfigureAwait(false);
            await DropItemResponseAsync(player, itemId, character, response);
        }

        private async Task InventoryDropItemAsync(IStrefaPlayer player, int inventoryId, int itemId, int amount)
        {
            if (!player.TryGetCharacter(out var character)) return;

            var inventory = InventoriesHelper.GetCorrectInventory(player, character, inventoryId);
            var response = InventoryDropResponse.ItemNotFound;
            if (inventory != null)
            {
                response = await inventory.DropItemAsync(itemId, amount, player.Position, _inventoriesManager, _inventoryDatabaseService).ConfigureAwait(false);
            }

            await DropItemResponseAsync(player, itemId, character, response).ConfigureAwait(false);
        }

        public async Task InventoryRemoveItemAsync(IPlayer player, int id, int amount)
        {
            if (!player.TryGetCharacter(out var character)) return;
            var response = await character.Inventory.RemoveItemAsync(id, amount, _inventoryDatabaseService);
            switch (response)
            {
                case InventoryRemoveResponse.ItemRemovedCompletly:
                    _logger.LogDebug("Item ID {itemId} should be removed from database", id);
                    break;
                case InventoryRemoveResponse.NotEnoughItems:
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Posiadasz za mało przedmiotów");
                    break;
                case InventoryRemoveResponse.ItemNotFound:
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz takiego przedmiotu.", 5500);
                    break;
                case InventoryRemoveResponse.ItemRemoved:
                    await _notificationService.ShowSuccessNotificationAsync(player, "Usunięto przedmiot", "Pomyślnie usunięto przedmiot.");
                    break;
            }
        }

        private async Task PickupDroppedItemAsync(IPlayer player, int networkItemId, int droppedItemId)
        {
            if (!player.TryGetCharacter(out var character)) return;
            if (!_inventoriesManager.TryGetDroppedItem(networkItemId, droppedItemId, out var droppedItem)) return;
            var response = await character.Inventory.AddItemAsync(droppedItem.Item, droppedItem.Count, _inventoryDatabaseService);
            if (!response.AnyChangesMade)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się podnieść przedmiotu");
                return;
            }
            await _inventoriesManager.RemoveDroppedItemAsync(droppedItem, networkItemId, response.ItemsAddedCount);
        }

        private void InventoryTryStackItem(IStrefaPlayer player, int inventoryId, int itemToStackFromId, int itemToStackId)
        {
            if (!player.TryGetCharacter(out var character)) return;

            var inventory = InventoriesHelper.GetCorrectInventory(player, character, inventoryId);
            var response = new InventoryStackResponse();

            if (inventory != null)
            {
                response = inventory.StackItem(itemToStackFromId, itemToStackId);
            }

            StackItemResponse(player, response);
        }
        
        private async Task InventoryTryStackItemBetweenInventoriesAsync(IStrefaPlayer player, int inventoryId, int itemToStackFromId, int itemToStackId, 
            int itemToStackInventoryId)
        {
            if (!player.TryGetCharacter(out var character)) return;

            var inventory = InventoriesHelper.GetCorrectInventory(player, character, inventoryId);
            var inventoryToStack = InventoriesHelper.GetCorrectInventory(player, character, itemToStackInventoryId);

            if (inventory == null || inventoryToStack == null)
            {
                await player.EmitAsync("inventoryStackItemResponse", false);
                return;
            }

            var response = await _inventoryTransferService.StackItemBetweenInventoriesAsync(inventory, inventoryToStack, itemToStackFromId, itemToStackId)
                .ConfigureAwait(false);
            await StackItemResponseAsync(player, response).ConfigureAwait(false);
        }

        private async Task InventoryTryUnequipItemAsync(IStrefaPlayer player, int playerEquipmentId, int inventoryId, int equippedItemId, int newSlotId)
        {
            if (!player.TryGetCharacter(out var character)) return;
            
            var inventory = InventoriesHelper.GetCorrectInventory(player, character, inventoryId);
            var response = await _inventoryEquipService.UnequipItemAsync((InventoryContainer)inventory, character, playerEquipmentId, equippedItemId, newSlotId);
            
            TryUnequipResponse(player, response);
        }

        private async Task InventoryTryEquipItemAsync(IStrefaPlayer player, int selectedInventoryId, int playerEquipmentId, int itemToEquipId)
        {
            if (!player.TryGetCharacter(out var character)) return;

            var inventory = InventoriesHelper.GetCorrectInventory(player, character, selectedInventoryId);
            var response = await _inventoryEquipService.EquipItemAsync(character, (InventoryContainer)inventory, playerEquipmentId, itemToEquipId);

            TryEquipResponse(player, playerEquipmentId, character, response);
        }

        private async Task StackItemResponseAsync(IPlayer player, InventoryStackResponse response)
        {
            switch (response.Type)
            {
                case InventoryStackResponseType.ItemsStacked:
                    await player.EmitAsync("inventoryStackItemResponse", true, response.AmountOfStackedItems);
                    break;
                case InventoryStackResponseType.ItemsNotFound:
                    await _notificationService.ShowErrorNotificationAsync(player, "Brak przedmiotu", "Wystąpił błąd i nie znaleziono takiego przedmiotu", 5000);
                    break;
                case InventoryStackResponseType.ItemsNotStackable:
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie można połączyć tych przedmiotów");
                    break;
            }
        }

        private void StackItemResponse(IPlayer player, InventoryStackResponse response)
        {
            switch (response.Type)
            {
                case InventoryStackResponseType.ItemsStacked:
                    player.Emit("inventoryStackItemResponse", true, response.AmountOfStackedItems);
                    break;
                case InventoryStackResponseType.ItemsNotFound:
                    _notificationService.ShowErrorNotification(player, "Brak przedmiotu", "Wystąpił błąd i nie znaleziono takiego przedmiotu", 5000);
                    break;
                case InventoryStackResponseType.ItemsNotStackable:
                    _notificationService.ShowErrorNotification(player, "Błąd", "Nie można połączyć tych przedmiotów");
                    break;
            }
        }

        private async Task DropItemResponseAsync(IPlayer player, int itemId, Character character, InventoryDropResponse response)
        {
            switch (response)
            {
                case InventoryDropResponse.NotEnoughItems:
                case InventoryDropResponse.ItemNotFound:
                case InventoryDropResponse.ItemNotDroppable:
                case InventoryDropResponse.ItemAlreadyDropped:
                    await player.EmitAsync("inventoryItemDropResponse", false, itemId);
                    break;
                case InventoryDropResponse.DroppedItem:
                    await player.EmitAsync("inventoryItemDropResponse", true, itemId);
                    _logger.LogInformation("Character {characterName} CID({characterId}) dropped item ID({itemId})", character.GetFullName(), character.Id, itemId);
                    break;
            }
        }

        private void TryEquipResponse(IStrefaPlayer player, int playerEquipmentId, Character character, InventoryEquipItemResponse response)
        {
            switch (response)
            {
                case InventoryEquipItemResponse.EquipmentInventoryNotFound:
                    player.EmitLocked("inventoryTryEquipItemResponse", false);
                    _logger.LogError("Not found player equipment inventory with ID({playerEquipmentInventoryId}) for character {characterName} CID({characterId})",
                        playerEquipmentId, character.GetFullName(), character.Id);
                    break;
                case InventoryEquipItemResponse.ItemAlreadyEquippedAtThatSlot:
                    _notificationService.ShowErrorNotification(player, "Błąd", "Jakiś przedmiot jest już założony na tym miejscu");
                    player.EmitLocked("inventoryTryEquipItemResponse", false);
                    break;
                case InventoryEquipItemResponse.CouldntEquipItem:
                case InventoryEquipItemResponse.ItemNotEquipmentable:
                case InventoryEquipItemResponse.ItemNotFound:
                    player.EmitLocked("inventoryTryEquipItemResponse", false);
                    break;
                case InventoryEquipItemResponse.ItemEquipped:
                    player.EmitLocked("inventoryTryEquipItemResponse", true);
                    break;
            }
        }

        
        private void TryUnequipResponse(IStrefaPlayer player, InventoryUnequipItemResponse response)
        {
            switch (response)
            {
                case InventoryUnequipItemResponse.ItemNotFound:
                    _notificationService.ShowErrorNotification(player, "Błąd", "Nie znaleziono takiego przedmiotu");
                    player.EmitLocked("inventoryTryEquipItemResponse", false);
                    break;
                case InventoryUnequipItemResponse.EquipmentInventoryNotFound:
                case InventoryUnequipItemResponse.ItemNotEquipmentable:
                case InventoryUnequipItemResponse.NoItemAtThatSlot:
                case InventoryUnequipItemResponse.InventoryIsFull:
                    player.EmitLocked("inventoryTryUnequipItemResponse", false);
                    break;
                case InventoryUnequipItemResponse.ItemUnequipped:
                    player.EmitLocked("inventoryTryUnequipItemResponse", true);
                    break;
            }
        }
    }
}
