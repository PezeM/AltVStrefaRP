using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Core;

namespace AltVStrefaRPServer.Services.MValueAdapters
{
    public class BlipWrapperAdapter : IMValueAdapter<BlipWrapper>
    {
        private readonly IMValueAdapter<Position> _positionAdapter;

        public BlipWrapperAdapter()
        {
            _positionAdapter = new PositionAdapter();
        }

        public BlipWrapper FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            int id = 0;
            string name = null;
            int color = 0;
            int sprite = 0;
            int type = 0;
            Position position = Position.Zero;
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "id":
                        id = reader.NextInt();
                        break;
                    case "name":
                        name = reader.NextString();
                        break;
                    case "color":
                        color = reader.NextInt();
                        break;
                    case "sprite":
                        sprite = reader.NextInt();
                        break;
                    case "type":
                        type = reader.NextInt();
                        break;
                    case "position":
                        position = _positionAdapter.FromMValue(reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return id == 0 ? null : new BlipWrapper(id, name, sprite, color, position, type);
        }

        public void ToMValue(BlipWrapper value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("name");
            writer.Value(value.Name);
            writer.Name("color");
            writer.Value(value.Color);
            writer.Name("sprite");
            writer.Value(value.Sprite);
            writer.Name("position");
            _positionAdapter.ToMValue(value.Position, writer); // Don't know if it will work
            writer.Name("type");
            writer.Value(value.Type);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer)
        {
            if (obj is BlipWrapper value)
            {
                ToMValue(value, writer);
            }
        }

        object IMValueBaseAdapter.FromMValue(IMValueReader reader)
        {
            return FromMValue(reader);
        }
    }

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
