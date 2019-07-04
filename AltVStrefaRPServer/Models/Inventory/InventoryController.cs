using System.Collections.Generic;
using System.Linq;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class InventoryController
    {
        public IReadOnlyCollection<InventoryItem> Items => _items;
        private List<InventoryItem> _items;

        public bool UseItem(Character character, int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                if (item.Item.UseItem(character))
                {
                    item.Quantity--;
                    if (item.Quantity <= 0)
                    {
                        _items.Remove(item);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool RemoveItem(int id, int amount)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item.Quantity < amount) return false;
            item.Quantity -= amount;
            if (item.Quantity <= 0)
            {
                _items.Remove(item);
            }
            return true;
        }

        public bool HasItem(int id, out InventoryItem item)
        {
            item = _items.FirstOrDefault(i => i.Id == id);
            return item != null;
        }
    }
}
