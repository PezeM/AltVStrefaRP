using System.Collections.Generic;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class PlayerEquipment : Inventory, IPlayerEquipment
    {
        public Character Owner { get; set; }

        public Dictionary<EquipmentSlot, InventoryItem> EquippedItems { get; private set; }

        public PlayerEquipment()
        {
            EquippedItems = new Dictionary<EquipmentSlot, InventoryItem>();
        }

        public void InitializeEquipment()
        {
            foreach (InventoryItem item in _items)
            {
                EquippedItems.Add((EquipmentSlot)item.SlotId, item);
            }
        }

        public InventoryEquipItemResponse EquipItem(InventoryItem inventoryItem)
        {
            if (!(inventoryItem.Item is Equipmentable equipmentableItem)) return InventoryEquipItemResponse.ItemNotEquipmentable;
            if (EquippedItems.ContainsKey(equipmentableItem.EquipmentSlot)) 
            {
                return InventoryEquipItemResponse.ItemAlreadyEquippedAtThatSlot;
            }

            if (!inventoryItem.Item.UseItem(Owner))
            {
                return InventoryEquipItemResponse.CouldntEquipItem;
            }

            inventoryItem.SetSlot((int)equipmentableItem.EquipmentSlot);
            AddInventoryItem(inventoryItem);
            EquippedItems[equipmentableItem.EquipmentSlot] = inventoryItem;
            return InventoryEquipItemResponse.ItemEquipped;
        }

        public InventoryUnequipItemResponse UnequipItem(EquipmentSlot slot)
        {
            if (!EquippedItems.TryGetValue(slot, out var inventoryItem)) return InventoryUnequipItemResponse.NoItemAtThatSlot;
            return UnequipItem(inventoryItem);
        }

        public InventoryUnequipItemResponse UnequipItem(InventoryItem inventoryItem)
        {
            if (!(inventoryItem.Item is Equipmentable equipmentableItem)) return InventoryUnequipItemResponse.ItemNotEquipmentable;
            if (!EquippedItems.ContainsKey(equipmentableItem.EquipmentSlot)) 
            {
                return InventoryUnequipItemResponse.NoItemAtThatSlot;
            }

            EquippedItems.Remove(equipmentableItem.EquipmentSlot);
            RemoveItem(inventoryItem);

            return InventoryUnequipItemResponse.ItemUnequipped;
        }
    }
}
