using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class PlayerInventoryController : InventoryController, IInventoryOwner<Character, PlayerInventoryController>
    {
        public Character Owner { get; set; }

        public IReadOnlyCollection<InventoryItem> EquippedItems => _equippedItems;
        private List<InventoryItem> _equippedItems;

        protected PlayerInventoryController() : base()
        {
            _equippedItems = new List<InventoryItem>();
        }

        public PlayerInventoryController(int maxSlots) : base()
        {
            _equippedItems = new List<InventoryItem>();
            MaxSlots = maxSlots;
        }

        public void TestEquip(Character character, int inventoryItemId)
        {
            if(!HasItem(inventoryItemId, out InventoryItem inventoryItem)) return;
            if (!(inventoryItem.Item is Equipmentable equipmentable)) return;
            if (_equippedItems.Any(i => i.SlotId == (int)equipmentable.EquipmentSlot)) return;
            inventoryItem.Item.UseItem(character);
            _items.Remove(inventoryItem);
            inventoryItem.SetSlot((int)equipmentable.EquipmentSlot);
            _equippedItems.Add(inventoryItem);
        }

        public void TestUnequip(Character character, int equippedItemId)
        {
            if (!_equippedItems.Any(i => i.Id == equippedItemId)) return;
            if(!HasEmptySlots()) return;
            var itemToUnequip = _equippedItems.First(i => i.Id == equippedItemId);
            _equippedItems.Remove(itemToUnequip);
            itemToUnequip.SetSlot(GetFreeSlot());
            _items.Add(itemToUnequip);
            character.Player.Emit("unequippedItem", equippedItemId, itemToUnequip.SlotId);
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
            var item = GetInventoryItem(itemId);
            if (item != null)
            {
                return await UseItemAsync(character, item, inventoryDatabaseService);
            }
            return InventoryUseResponse.ItemNotFound;
        }

        public bool TryToGetEquippedItemAtSlot(EquipmentSlot slot, out InventoryItem item)
        {
            item = _equippedItems.FirstOrDefault(i => i.SlotId == (int)slot);
            return true;
        }

        private void Test()
        {
            var item = new EquippedItem<ClothItem>();
        }
    }

    public class InventoryItem<T> where T : BaseItem
    {
        public int Id { get; set; }
        public T Item { get; set; }
        public int Quantity { get; set; }
    }

    public class EquippedItem<T> : InventoryItem<T> where T : Equipmentable
    {
        public EquipmentSlot Slot { get; set; }
    }
}
