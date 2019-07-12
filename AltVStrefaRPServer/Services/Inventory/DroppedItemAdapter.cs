using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Services.Inventory
{
    public class DroppedItemAdapter : IMValueAdapter<DroppedItem>
    {
        public DroppedItem FromMValue(IMValueReader reader)
        { 
            reader.BeginObject();
            int id = 0;
            string model = string.Empty;
            int count = 0;
            double x = 0;
            double y = 0;
            double z = 0;
            while (reader.HasNext())
            { 
                switch (reader.NextName())
                {
                    case "id":
                        id = reader.NextInt();
                        break;
                    case "model":
                        model = reader.NextString();
                        break;
                    case "count":
                        count = reader.NextInt();
                        break;
                    case "x":
                        x = reader.NextDouble();
                        break;
                    case "y":
                        y = reader.NextDouble();
                        break;
                    case "z":
                        z = reader.NextDouble();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return id == 0 ? null : new DroppedItem
            {
                Count = count,
                Id = id,
                Model = model,
                X = (float)x,
                Y = (float)y,
                Z = (float)z
            };
        }

        public void ToMValue(DroppedItem value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("name");
            writer.Value(value.Name);
            writer.Name("model");
            writer.Value(value.Model);
            writer.Name("count");
            writer.Value(value.Count);
            writer.Name("x");
            writer.Value(value.X);
            writer.Name("y");
            writer.Value(value.Y);
            writer.Name("z");
            writer.Value(value.Z);
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
