using AltV.Net;

namespace AltVStrefaRPServer.Models.Dto.Fractions
{
    public class TaxDto : IMValueConvertible
    {
        private static readonly IMValueBaseAdapter _myAdapter = new TaxDtoAdapter();
        public int Id { get; set; }
        public float Value { get; set; }
        public string Name { get; set; }

        public TaxDto(int id, string name, float value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        public IMValueBaseAdapter GetAdapter() => _myAdapter;
    }

    public class TaxDtoAdapter : IMValueAdapter<TaxDto>
    {
        public TaxDto FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            int id = 0;
            string name = null;
            float value = 0;
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
                    case "value":
                        value = (float)reader.NextDouble();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return id == 0 ? null : new TaxDto(id, name, value);
        }

        public void ToMValue(TaxDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("value");
            writer.Value(value.Value);
            writer.Name("name");
            writer.Value(value.Name);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer)
        {
            if (obj is TaxDto value)
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
