using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models.Inventory;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Services.Inventory
{
    public class InventoryDatabaseService : IInventoryDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public InventoryDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<DroppedItem> GetAllDroppedItems()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<InventoryItem> GetAllInventoryItems()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BaseItem> GetAllItems()
        {
            throw new NotImplementedException();
        }

        public InventoryItem GetInventoryItem(int id)
        {
            throw new NotImplementedException();
        }

        public BaseItem GetItem(int id)
        {
            throw new NotImplementedException();
        }

        public void SaveItem(BaseItem item)
        {
            throw new NotImplementedException();
        }

        public Task SaveItemAsync(BaseItem item)
        {
            throw new NotImplementedException();
        }
    }
}
