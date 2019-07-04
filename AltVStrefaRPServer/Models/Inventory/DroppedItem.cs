using System;
using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Services.Inventory;

namespace AltVStrefaRPServer.Models.Inventory
{
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
        public DroppedItem(int id, int count, string model, BaseItem item, Position position)
        {
            Id = id;
            Count = count;
            Item = item;
            Model = model;
            Position = position;
            RemoveTime = DateTime.Now.AddMinutes(5); // For testing now
        }

        public IMValueBaseAdapter GetAdapter() => ItemAdapters.DroppedItemAdapter;
    }
}
