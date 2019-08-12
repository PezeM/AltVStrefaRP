using System;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
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

        public async Task<InventoryEquipItemResponse> EquipItemAsync(Character character, IInventoryContainer inventory, int playerEquipmentId, int itemToEquip)
        {
            if (character.Equipment?.Id != playerEquipmentId)
                return InventoryEquipItemResponse.EquipmentInventoryNotFound;

            return await EquipItemAsync(inventory, character.Equipment, itemToEquip);
        }

        public async Task<InventoryEquipItemResponse> EquipItemAsync(IInventoryContainer inventory, IPlayerEquipment playerEquipment, int itemToEquipId)
        {
            using (var context = _factory.Invoke())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    if (!inventory.HasItem(itemToEquipId, out var itemToEquip) && itemToEquip.Quantity < 1) return InventoryEquipItemResponse.ItemNotFound;
                    
                    var response = playerEquipment.EquipItem(itemToEquip);
                    if (response != InventoryEquipItemResponse.ItemEquipped) return response;

                    context.Update(playerEquipment);
                    await context.SaveChangesAsync();

                    if (inventory.RemoveItem(itemToEquip, 1) == InventoryRemoveResponse.ItemRemovedCompletly)
                    {
                        context.Update(inventory);
                        await context.SaveChangesAsync();
                    }

                    transaction.Commit();
                    return InventoryEquipItemResponse.ItemEquipped;
                }
            }
        }
    }
}
