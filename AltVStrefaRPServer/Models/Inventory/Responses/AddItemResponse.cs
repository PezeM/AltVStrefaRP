using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Inventory.Responses
{
    public struct AddItemResponse
    {
        public bool AddedNewItem { get; set; }
        public int ItemsAddedCount { get; set; }
        public List<InventoryItem> NewItems { get; }

        public AddItemResponse(int itemsAddedCount = 0, bool addedNewItem = false)
        {
            ItemsAddedCount = itemsAddedCount;
            AddedNewItem = addedNewItem;
            NewItems = new List<InventoryItem>();
        }

        /// <summary>
        /// If any items were added to the inventory or any items were added to the stack
        /// Mostly false if there was no space in the inventory
        /// </summary>
        public bool AnyChangesMade => ItemsAddedCount > 0 || AddedNewItem;

    }
}
