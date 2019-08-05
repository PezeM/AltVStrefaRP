using System.Linq;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Models.Vehicles;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public static class InventoryContainerConverter
    {
        public static InventoryContainerDto ConvertFromCharacterInventory(Character character)
        {
            return new InventoryContainerDto
            {
                InventoryId = character.InventoryId,
                InventorySlots = character.Inventory.MaxSlots,
                InventoryName = "Ekwipunek",
                Items = character.Inventory.Items.ToList()
            };
        }

        public static InventoryContainerDto ConvertFromEquippedInventory(Character character)
        {
            return new InventoryContainerDto
            {
                InventoryId = character.InventoryId,
                InventorySlots = character.Inventory.MaxSlots,
                InventoryName = "Założone przedmioty",
                Items = character.Inventory.EquippedItems.ToList()
            };
        }

        public static InventoryContainerDto ConvertFromVehicleInventory(VehicleModel vehicle)
        {
            return new InventoryContainerDto()
            {
                InventoryId = vehicle.InventoryId,
                InventoryName = $"Bagażnik pojazdu {vehicle.PlateText}",
                InventorySlots = vehicle.Inventory.MaxSlots,
                Items = vehicle.Inventory.Items.ToList()
            };
        }
    }
}
