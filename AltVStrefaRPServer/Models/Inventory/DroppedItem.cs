using System;
using AltV.Net;
using AltVStrefaRPServer.Models.Interfaces;
using AltVStrefaRPServer.Models.Inventory.Items;
using AltVStrefaRPServer.Services.Inventory;
using Position = AltV.Net.Data.Position;

namespace AltVStrefaRPServer.Models.Inventory
{
    // Todo save to database
    public class DroppedItem : IMValueConvertible, IPosition
    {
        public int Id { get;set; }
        public string Name => Item.Name;
        public int Count { get; set; }
        public string Model { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public DateTime RemoveTime { get; set; }
        public BaseItem Item { get; set; }
        public int BaseItemId { get; protected set; }

        public DroppedItem(){}

        public DroppedItem(int count, string model, BaseItem item, Position position)
        {
            Count = count;
            Item = item;
            Model = model;
            X = position.X;
            Y = position.Y;
            Z = position.Z;
            RemoveTime = DateTime.Now.AddMinutes(5); // For testing now
        }

        public DroppedItem(int id, int count, string model, BaseItem item, Position position) : this(count, model, item, position)
        {
            Id = id;
        }

        public IMValueBaseAdapter GetAdapter() => ItemAdapters.DroppedItemAdapter;

        public Position GetPosition()
        {
            return new Position(X,Y,Z);
        }
    }
}
