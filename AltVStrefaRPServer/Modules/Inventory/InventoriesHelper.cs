using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Interfaces;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoriesHelper
    {
        /// <summary>
        /// Decides if <see cref="inventoryId"/> is character own inventory or player last opened inventory
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="character">Character</param>
        /// <param name="inventoryId">Id of the inventory we want to check</param>
        /// <returns>Returns <see cref="IInventoryController"/> if found inventory otherwise <see cref="null"/></returns>
        public static IInventoryController GetCorrectInventory(IStrefaPlayer player, Character character, int inventoryId)
        {
            if (character.InventoryId == inventoryId) return character.Inventory;
            else if (player.LastOpenedInventory?.Id == inventoryId) return player.LastOpenedInventory;
            return null;
        }

        /// <summary>
        /// Checks if items are stackable
        /// </summary>
        /// <param name="itemToStackFrom">The item we want to stack with</param>
        /// <param name="itemToStack">The item we want to stack</param>
        /// <returns>True if items are stackable</returns>
        public static bool AreItemsStackable(InventoryItem itemToStackFrom, InventoryItem itemToStack) => itemToStackFrom.Item.Name == itemToStack.Item.Name;
    }
}
