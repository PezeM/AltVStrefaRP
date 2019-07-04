using System;
using System.Collections.Concurrent;
using System.Linq;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Modules.Inventory
{
    public class InventoryManager
    {
        public ConcurrentDictionary<int, DroppedItem> DroppedItems { get; set; }
        public ConcurrentDictionary<int, InventoryItem> Items { get; set; }

        private Func<ServerContext> _factory;

        public InventoryManager(Func<ServerContext> factory)
        {
            DroppedItems = new ConcurrentDictionary<int, DroppedItem>();
            Items = new ConcurrentDictionary<int, InventoryItem>();

            InitializeInventory();
        }

        private void InitializeInventory()
        {
            using (var context = _factory.Invoke())
            {
                var itemsToRemove = context.BankAccounts.Where(q => q.AccountNumber > 1000);
                if (itemsToRemove.Count() > 0)
                {
                    context.RemoveRange(itemsToRemove);
                    context.SaveChanges();
                }
            }
        }
    }
}
