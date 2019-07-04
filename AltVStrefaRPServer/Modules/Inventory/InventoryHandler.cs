using System;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Models.Interfaces.Inventory;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoryHandler
    {
        private readonly InventoryManager _inventoryManager;
        public InventoryHandler(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
            Alt.On<IPlayer, int, int, Position, Rotation>("DropItem", DropItem);
        }

        private void DropItem(IPlayer player, int id, int amount, Position position, Rotation rotation)
        {
            if (!player.TryGetCharacter(out var charactr)) return;
            //if (!charactr.Inventory.HasItem(id, out var item)) return;
            //if (item is IDroppable droppable)
            //{
            //    if (!charactr.Inventory.RemoveItem(item.Id, amount)) return;
            //    // Add new method that will override adding to dictionary, send event to all player that item dropped on the ground etc
            //    _inventoryManager.DroppedItems.TryAdd(item.Id, new DroppedItem(item.Id, item.Item, position, rotation, DateTime.Today.AddHours(2)));
            //}
        }
    }
}
