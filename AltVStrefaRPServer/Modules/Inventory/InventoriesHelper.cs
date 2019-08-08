using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Inventory;

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
    }
}
