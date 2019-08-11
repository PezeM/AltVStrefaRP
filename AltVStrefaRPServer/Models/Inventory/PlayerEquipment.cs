using System.Collections.Generic;
using AltV.Net;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class PlayerEquipment : Inventory, IPlayerEquipment
    {
        public Character Owner { get; set; }

        public Dictionary<EquipmentSlot, InventoryItem> EquippedItems { get; private set; }

        public PlayerEquipment()
        {
            EquippedItems = new Dictionary<EquipmentSlot, InventoryItem>();
            foreach (InventoryItem item in _items)
            {
                EquippedItems.Add((EquipmentSlot)item.SlotId, item);
            }
        }

        public void EquipItem(int itemId)
        {
            if (!HasItem(itemId, out var inventoryItem)) return;
            EquipItem(inventoryItem);
        }

        public void EquipItem(InventoryItem inventoryItem)
        {
            if (!(inventoryItem.Item is Equipmentable equipmentableItem)) return;
            if (EquippedItems.ContainsKey(equipmentableItem.EquipmentSlot)) 
            {
                Alt.Log("Jest juz item na tym slocie");
                return;
            }

            inventoryItem.Item.UseItem(Owner);

            inventoryItem.SetSlot((int)equipmentableItem.EquipmentSlot);
            AddItem(inventoryItem);
            EquippedItems[equipmentableItem.EquipmentSlot] = inventoryItem;
        }

        public void UnequipItem(int itemId)
        {
            throw new System.NotImplementedException();
        }

        public void UnequipItem(InventoryItem item)
        {
            throw new System.NotImplementedException();
        }
    }
}
