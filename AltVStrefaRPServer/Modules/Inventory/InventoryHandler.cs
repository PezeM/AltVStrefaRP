using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoryHandler
    {
        private readonly InventoryManager _inventoryManager;
        private readonly INotificationService _notificationService;

        public InventoryHandler(InventoryManager inventoryManager, INotificationService notificationService)
        {
            _inventoryManager = inventoryManager;
            _notificationService = notificationService;

            Alt.On<IPlayer, int, int, Position, Rotation>("DropItem", DropItem);
        }

        private void DropItem(IPlayer player, int id, int amount, Position position, Rotation rotation)
        {
            if (!player.TryGetCharacter(out var character)) return;
            var response = character.Inventory.DropItem(id, amount, position, _inventoryManager);
            switch (response)
            {
                case InventoryDropResponse.ItemNotDroppable:
                    _notificationService.ShowErrorNotification(player, "Błąd", "Nie da się wyrzucić tego przedmiotu.");
                    break;
                case InventoryDropResponse.ItemNotFound:
                    _notificationService.ShowErrorNotification(player, "Błąd", "Nie posiadasz tego przedmiotu.");
                    break;
                case InventoryDropResponse.NotEnoughItems:
                    _notificationService.ShowErrorNotification(player, "Błąd", "Nie masz wystarczającej ilości przedmiotu.");
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
    }
}
