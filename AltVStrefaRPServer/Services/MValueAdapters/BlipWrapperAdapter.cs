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
            float scale = 0;
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
                    case "scale":
                        scale = (float)reader.NextDouble();
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
            return id == 0 ? null : new BlipWrapper(id, name, sprite, color, position, scale);
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
            writer.Name("scale");
            writer.Value(value.Scale);
            writer.Name("position");
            _positionAdapter.ToMValue(value.Position, writer); // Don't know if it will work
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
}
