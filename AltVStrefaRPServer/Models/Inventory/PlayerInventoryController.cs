using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class PlayerInventoryController : InventoryController, IInventoryOwner<Character, PlayerInventoryController>
    {
        public Character Owner { get; set; }

        public IReadOnlyCollection<InventoryItem> EquippedItemsList => _equippedItemsList;
        private List<InventoryItem> _equippedItemsList;

        public Dictionary<EquipmentSlot, InventoryItem> EquippedItems { get; private set; }

        public void Test()
        {
            EquippedItems = new Dictionary<EquipmentSlot, InventoryItem>();
            foreach (InventoryItem item in _equippedItemsList)
            {
                EquippedItems.Add((EquipmentSlot)item.SlotId, item);
            }
        }

        protected PlayerInventoryController()
        {
            _equippedItemsList = new List<InventoryItem>();
            Test();
        }

        public PlayerInventoryController(int maxSlots) : this()
        {
            MaxSlots = maxSlots;
        }

        public void TestEquipItem(Character character, int itemId)
        {
            if (!HasItem(itemId, out var inventoryItem)) return;
            if (!(inventoryItem.Item is Equipmentable equipmentableItem)) return;
            if (EquippedItems.ContainsKey(equipmentableItem.EquipmentSlot)) 
            {
                Alt.Log("Jest juz item na tym slocie");
                return;
            }

            if (UseItem(character, inventoryItem, inventoryItem.Quantity) == InventoryUseResponse.ItemNotUsed) return;

            inventoryItem.SetSlot((int)equipmentableItem.EquipmentSlot);
            _equippedItemsList.Add(inventoryItem);
            EquippedItems[equipmentableItem.EquipmentSlot] = inventoryItem;
        }

        public void TestUnequipItem(Character character, int itemId)
        {
            if (!HasItem(itemId, out var inventoryItem)) return;
            if (!(inventoryItem.Item is Equipmentable equipmentableItem)) return;
            if (EquippedItems.ContainsKey(equipmentableItem.EquipmentSlot)) 
            {
                Alt.Log("Jest juz item na tym slocie");
                return;
            }

            if (UseItem(character, inventoryItem, inventoryItem.Quantity) == InventoryUseResponse.ItemNotUsed) return;

            inventoryItem.SetSlot((int)equipmentableItem.EquipmentSlot);
            _equippedItemsList.Add(inventoryItem);
            EquippedItems[equipmentableItem.EquipmentSlot] = inventoryItem;
        }

        public InventoryUseResponse UseItem(Character character, int itemId, int quantity = 1)
        {
            if (!HasItem(itemId, out var item)) return InventoryUseResponse.ItemNotFound;
            return UseItem(character, item, quantity);
        }

        public InventoryUseResponse UseItem(Character character, InventoryItem item, int quantity = 1)
        {
            if (!item.Item.UseItem(character)) return InventoryUseResponse.ItemNotUsed;
            item.RemoveQuantity(quantity);
            if (item.Quantity <= 0)
            {
                _items.Remove(item);
            }
            return InventoryUseResponse.ItemUsed;
        }

        public async Task<InventoryUseResponse> UseItemAsync(Character character, InventoryItem item, IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!item.Item.UseItem(character)) return InventoryUseResponse.ItemNotUsed;
            item.RemoveQuantity(1);
            if (item.Quantity <= 0)
            {
                _items.Remove(item);
                await inventoryDatabaseService.RemoveItemAsync(item);
            }
            return InventoryUseResponse.ItemUsed;
        }

        public async Task<InventoryUseResponse> UseItemAsync(Character character, int itemId, IInventoryDatabaseService inventoryDatabaseService)
        {
            if (!HasItem(itemId, out var item)) return InventoryUseResponse.ItemNotFound;
            return await UseItemAsync(character, item, inventoryDatabaseService);
        }
    }
}
