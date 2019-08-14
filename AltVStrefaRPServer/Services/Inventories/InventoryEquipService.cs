using System;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Responses;

namespace AltVStrefaRPServer.Services.Inventories
{
    public class InventoryEquipService : IInventoryEquipService
    {
        private readonly Func<ServerContext> _factory;

        public InventoryEquipService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public async Task<InventoryEquipItemResponse> EquipItemAsync(Character character, InventoryContainer inventory, int playerEquipmentId, int itemToEquip)
        {
            if (character.Equipment?.Id != playerEquipmentId)
                return InventoryEquipItemResponse.EquipmentInventoryNotFound;

            return await EquipItemAsync(inventory, character.Equipment, itemToEquip);
        }

        public async Task<InventoryEquipItemResponse> EquipItemAsync(InventoryContainer inventory, PlayerEquipment playerEquipment, int itemToEquipId)
        {
            using (var context = _factory.Invoke())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    if (!inventory.HasItem(itemToEquipId, out var itemToEquip) || itemToEquip.Quantity < 1) return InventoryEquipItemResponse.ItemNotFound;
                    
                    var response = playerEquipment.EquipItem(itemToEquip);
                    if (response != InventoryEquipItemResponse.ItemEquipped) return response;

                    if (inventory.RemoveItem(itemToEquip))
                    {
                        context.InventoryContainers.Update(inventory);
                        await context.SaveChangesAsync();
                    }

                    context.PlayerEquipments.Update(playerEquipment);
                    await context.SaveChangesAsync();

                    transaction.Commit();
                    return InventoryEquipItemResponse.ItemEquipped;
                }
            }
        }

        public async Task<InventoryUnequipItemResponse> UnequipItemAsync(InventoryContainer inventory, PlayerEquipment playerEquipment, EquipmentSlot slotId)
        {
            if (!playerEquipment.EquippedItems.TryGetValue(slotId, out var itemToUnequip)) return InventoryUnequipItemResponse.ItemNotFound;
            return await UnequipItemAsync(inventory, playerEquipment, itemToUnequip.Id);
        }

        public async Task<InventoryUnequipItemResponse> UnequipItemAsync(InventoryContainer inventory, Character character, int playerEquipmentId,
            int itemToEquipId, int newSlotId)
        {
            if (character.Equipment?.Id != playerEquipmentId)
                return InventoryUnequipItemResponse.EquipmentInventoryNotFound;

            return await UnequipItemAsync(inventory, character.Equipment, itemToEquipId, newSlotId);
        }

        public async Task<InventoryUnequipItemResponse> UnequipItemAsync(InventoryContainer inventory, PlayerEquipment playerEquipment, int itemToUnequipId, 
            int newSlotId = -1)
        {
            if (!playerEquipment.HasItem(itemToUnequipId, out var itemToUnequip)) return InventoryUnequipItemResponse.ItemNotFound;
            return await UnequipItemAsync(inventory, playerEquipment, itemToUnequip, newSlotId);
        }

        public async Task<InventoryUnequipItemResponse> UnequipItemAsync(InventoryContainer inventory, PlayerEquipment playerEquipment, InventoryItem itemToUnequip, 
            int newSlotId = -1)
        {
            using (var context = _factory.Invoke())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    // If called by method with item id it's called multiple times, but we still have to make check if inventory contains item.. 
                    if (!playerEquipment.HasItem(itemToUnequip)) return InventoryUnequipItemResponse.ItemNotFound;
                    if (!inventory.HasEmptySlots()) return InventoryUnequipItemResponse.InventoryIsFull;
                    if (newSlotId > -1 && !inventory.IsSlotEmpty(newSlotId)) return InventoryUnequipItemResponse.InventoryIsFull;

                    var unequipItemResponse = playerEquipment.UnequipItem(itemToUnequip);
                    if (unequipItemResponse != InventoryUnequipItemResponse.ItemUnequipped) return unequipItemResponse;

                    if (newSlotId > -1)
                        inventory.AddInventoryItem(itemToUnequip, newSlotId);
                    else
                        inventory.AddInventoryItem(itemToUnequip);

                    context.PlayerEquipments.Update(playerEquipment);
                    context.InventoryContainers.Update(inventory);
                    await context.SaveChangesAsync();

                    transaction.Commit();
                    return InventoryUnequipItemResponse.ItemUnequipped;
                }
            }
        }
    }
}
