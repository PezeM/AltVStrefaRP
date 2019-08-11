using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Responses;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class PlayerInventoryContainer : InventoryContainer, IInventoryOwner<Character, PlayerInventoryContainer>
    {
        public Character Owner { get; set; }

        protected PlayerInventoryContainer() {}

        public PlayerInventoryContainer(int maxSlots) : base(maxSlots)
        {
            MaxSlots = maxSlots;
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
