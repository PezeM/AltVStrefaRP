using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Inventory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Position = AltV.Net.Data.Position;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoryHandler
    {
        private readonly IInventoriesManager _inventoriesManager;
        private readonly IInventoryDatabaseService _inventoryDatabaseService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<InventoryHandler> _logger;

        public InventoryHandler(IInventoriesManager inventoriesManager, IInventoryDatabaseService inventoryDatabaseService, INotificationService notificationService, 
            ILogger<InventoryHandler> logger)
        {
            _inventoriesManager = inventoriesManager;
            _inventoryDatabaseService = inventoryDatabaseService;
            _notificationService = notificationService;
            _logger = logger;

            Alt.On<IPlayer>("GetPlayerInventory", GetPlayerInventory);
            AltAsync.On<IPlayer, int, int, Position, Task>("DropItem", DropItemAsync);
            AltAsync.On<IPlayer, int, Task>("UseInventoryItem",UseInventoryItemAsync);
            AltAsync.On<IPlayer, int, int, Task>("InventoryDropItem", InventoryDropItemAsync);
            AltAsync.On<IPlayer, int, int, Task>("InventoryRemoveItem", InventoryRemoveItemAsync);
            AltAsync.On<IPlayer, int, int, Task>("PickupDroppedItem", PickupDroppedItemAsync);
        }

        private void GetPlayerInventory(IPlayer player)
        {
            var startTime = Time.GetTimestampMs();
            if(!player.TryGetCharacter(out var character)) return;
            var inventoryItems = character.Inventory.Items.Select(i => new InventoryItemDto(i));
            player.Emit("populatePlayerInventory", JsonConvert.SerializeObject(inventoryItems), JsonConvert.SerializeObject(character.Inventory.EquippedItems));
            _logger.LogDebug("Send player inventory in {elapsedTime}", Time.GetElapsedTime(startTime));
        }

        public async Task DropItemAsync(IPlayer player, int id, int amount, Position position)
        {
            if (!player.TryGetCharacter(out var character)) return;
            var response = await character.Inventory.DropItemAsync(id, amount, position, _inventoriesManager, _inventoryDatabaseService);
            switch (response)
            {
                case InventoryDropResponse.ItemNotDroppable:
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie da się wyrzucić tego przedmiotu.");
                    break;
                case InventoryDropResponse.ItemNotFound:
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie posiadasz tego przedmiotu.");
                    break;
                case InventoryDropResponse.NotEnoughItems:
                    await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie masz wystarczającej ilości przedmiotu.");
                    break;
                case InventoryDropResponse.ItemAlreadyDropped:
                    _logger.LogDebug("Character CID({characterId}) wanted to drop item that was already dropped, item ID({itemId})", character.Id, id);
                    break;
                case InventoryDropResponse.DroppedItem:
                    // Propably update user inventory
                    // Remove item with id and ammount
                    break;
            }
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

        private async Task InventoryDropItemAsync(IPlayer player, int itemId, int amount)
        {
            if (!player.TryGetCharacter(out var character)) return;
            var response = await character.Inventory.DropItemAsync(itemId, amount, player.Position, _inventoriesManager, _inventoryDatabaseService);
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
                    break;
            }
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
            // Check if networking entity exists and there is dropped item with given id
            // Add items to inventory
            // If items were added to inventory remove them from dropped item list and remove networking entity

            // Make it possible to take only few quantity of item till full inventory.
            // Make it possible to decrease dropped item quantity - also decrease networking entity count
            if (!player.TryGetCharacter(out var character)) return;
            if (!_inventoriesManager.TryToGetDroppedItem(networkItemId, droppedItemId, out var droppedItem)) return;
            var response = await character.Inventory.AddItemAsync(droppedItem.Item, droppedItem.Count, _inventoryDatabaseService, player);
            if (!response.AnyChangesMade)
            {
                await _notificationService.ShowErrorNotificationAsync(player, "Błąd", "Nie udało się podnieść przedmiotu");
                return;
            }
            await _inventoriesManager.RemoveDroppedItemAsync(droppedItem, networkItemId, response.ItemsAddedCount);
        }
    }
}
