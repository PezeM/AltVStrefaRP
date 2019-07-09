using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services;
using AltVStrefaRPServer.Services.Inventory;
using Newtonsoft.Json;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoryHandler
    {
        private readonly InventoryManager _inventoryManager;
        private readonly IInventoryDatabaseService _inventoryDatabaseService;
        private readonly INotificationService _notificationService;

        public InventoryHandler(InventoryManager inventoryManager, IInventoryDatabaseService inventoryDatabaseService, INotificationService notificationService)
        {
            _inventoryManager = inventoryManager;
            _inventoryDatabaseService = inventoryDatabaseService;
            _notificationService = notificationService;

            AltAsync.On<IPlayer, int, int, Position>("DropItem", async (player, id, amount, position) 
                => await DropItem(player, id, amount, position));
            AltAsync.On<IPlayer, int>("UseInventoryItem", async (player, itemId) => await UseInventoryItem(player, itemId));
            Alt.On<IPlayer>("GetPlayerInventory", GetPlayerInventory);
        }

        private void GetPlayerInventory(IPlayer player)
        {
            var startTime = Time.GetTimestampMs();
            if(!player.TryGetCharacter(out var character)) return;
            var inventoryItems = character.Inventory.Items.Select(i => new InventoryItemDto(i));
            player.Emit("populatePlayerInventory", JsonConvert.SerializeObject(inventoryItems), JsonConvert.SerializeObject(character.Inventory.EquippedItems));
            Alt.Log($"Send player inventory in {Time.GetTimestampMs() - startTime}ms.");
        }

        public async Task DropItem(IPlayer player, int id, int amount, Position position)
        {
            if (!player.TryGetCharacter(out var character)) return;
            var response = await character.Inventory.DropItem(id, amount, position, _inventoryManager);
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
                    Alt.Log($"{character.Id} wanted to drop item that was already dropped. Item id {id}");
                    break;
                case InventoryDropResponse.DroppedItem:
                    // Propably update user inventory
                    // Remove item with id and ammount
                    break;
            }
        }

        public async Task UseInventoryItem(IPlayer player, int itemId)
        {
            if (!player.TryGetCharacter(out var character)) return;
            var response = await character.Inventory.UseItem(character, itemId, _inventoryDatabaseService);
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
    }
}
