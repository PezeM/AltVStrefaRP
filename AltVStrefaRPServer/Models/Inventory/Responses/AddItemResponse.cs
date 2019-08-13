using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Inventory.Responses
{
    public struct AddItemResponse
    {
        public int ItemsAddedCount { get; set; }
        public List<InventoryItem> NewItems { get; }

        public AddItemResponse(int itemsAddedCount = 0)
        {
            ItemsAddedCount = itemsAddedCount;
            NewItems = new List<InventoryItem>();
        }

        /// <summary>
        /// If any items were added to the inventory or any items were added to the stack
        /// Mostly false if there was no space in the inventory
        /// </summary>
        public bool AnyChangesMade => ItemsAddedCount > 0 || NewItems.Count > 0;

    }
}
