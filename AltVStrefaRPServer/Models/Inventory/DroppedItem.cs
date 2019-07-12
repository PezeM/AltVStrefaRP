using System;
using AltV.Net;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Services.Inventory;
using Position = AltV.Net.Data.Position;

namespace AltVStrefaRPServer.Models.Inventory
{
    // Todo save to database
    public class DroppedItem : IMValueConvertible
    {
        public int Id { get;set; }
        public string Name => Item.Name;
        public int Count { get; set; }
        public string Model { get; set; }
        public DateTime RemoveTime { get; set; }
        public Position Position { get; set; }
        public BaseItem Item { get; set; }

        public DroppedItem(){}

        public DroppedItem(int count, string model, BaseItem item, Position position)
        {
            Count = count;
            Item = item;
            Model = model;
            Position = position;
            RemoveTime = DateTime.Now.AddMinutes(5); // For testing now
        }

        public DroppedItem(int id, int count, string model, BaseItem item, Position position) : this(count, model, item, position)
        {
            Id = id;
        }

        public IMValueBaseAdapter GetAdapter() => ItemAdapters.DroppedItemAdapter;
    }
}
