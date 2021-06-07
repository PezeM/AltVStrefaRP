using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Core;

namespace AltVStrefaRPServer.Services.MValueAdapters
{
    public class PositionAdapter : IMValueAdapter<Position>
    {
        public Position FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            float x = 0;
            float y = 0;
            float z = 0;
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "x":
                        x = (float)reader.NextDouble();
                        break;
                    case "y":
                        y = (float)reader.NextDouble();
                        break;
                    case "z":
                        z = (float)reader.NextDouble();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            reader.EndObject();
            return new Position(x, y, z);
        }

        public void ToMValue(Position value, IMValueWriter writer)
        {
            writer.BeginObject();
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
            if (obj is Position value)
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
