﻿using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Models.Inventory.Interfaces
{
    public interface IPlayerEquipment : IInventory
    {
        Character Owner { get; }

        InventoryEquipItemResponse EquipItem(InventoryItem item);
        InventoryUnequipItemResponseType UnequipItem(EquipmentSlot slot);
        InventoryUnequipItemResponseType UnequipItem(InventoryItem inventoryItem);
    }
}
