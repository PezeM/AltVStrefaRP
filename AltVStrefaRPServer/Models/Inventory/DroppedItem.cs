using System;
using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Inventory.Items;

namespace AltVStrefaRPServer.Models.Inventory
{
    public class DroppedItem : IMValueConvertible
    {
        private static readonly IMValueBaseAdapter _myAdapter = new DroppedItemAdapter();
        public int Id { get;set; }
        public string Name => Item.Name;
        public DateTime RemoveTime { get; set; }
        public Position Position { get; set; }
        public Rotation Rotation { get; set; }
        public BaseItem Item { get; set; }

        protected DroppedItem(){}
        public DroppedItem(int id, BaseItem item, Position position, Rotation rotation, DateTime removeTime)
        {
            Id = id;
            Item = item;
            Position = position;
            Rotation = rotation;
            RemoveTime = removeTime;
        }

        public DroppedItem(int id)
        {
            Id = id;
            RemoveTime = DateTime.MaxValue;
        }

        public IMValueBaseAdapter GetAdapter() => _myAdapter;
    }

    public class DroppedItemAdapter : IMValueAdapter<DroppedItem>
    {
        public DroppedItem FromMValue(IMValueReader reader)
        { 
            reader.BeginObject();
            int id = 0;
            while (reader.HasNext())
            { 
                switch (reader.NextName())
                {
                    case "id":
                        id = reader.NextInt();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return id == 0 ? null : new DroppedItem(id);
        }

        public void ToMValue(DroppedItem value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("name");
            writer.Value(value.Name);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer) 
        { 
            if (obj is DroppedItem value) 
            { 
                ToMValue(value, writer);
            }
        }

        object IMValueBaseAdapter.FromMValue(IMValueReader reader) 
        { 
            return FromMValue(reader);
        }
    }
}
