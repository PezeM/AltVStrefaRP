using System;
using System.Collections.Concurrent;
using AltV.Net;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoryManager
    {
        private ConcurrentDictionary<int, DroppedItem> _droppedItems;
        private ConcurrentDictionary<int, InventoryItem> _items;

        private Func<ServerContext> _factory;

        public InventoryManager(Func<ServerContext> factory)
        {
            _droppedItems = new ConcurrentDictionary<int, DroppedItem>();
            _items = new ConcurrentDictionary<int, InventoryItem>();
            _factory = factory;

            LoadItems();
        }

        private void LoadItems()
        {
            using (var context = _factory.Invoke())
            {
                //var itemsToRemove = context.BankAccounts.Where(q => q.AccountNumber > 1000);
                //if (itemsToRemove.Count() > 0)
                //{
                //    context.RemoveRange(itemsToRemove);
                //    context.SaveChanges();
                //}
            }
        }

        public bool AddDroppedItem(DroppedItem droppedItem)
        {
            if (!_droppedItems.TryAdd(droppedItem.Id, droppedItem)) return false;
            Alt.EmitAllClients("streamInDroppedItem", droppedItem);
            return true;
        }
    }
}
