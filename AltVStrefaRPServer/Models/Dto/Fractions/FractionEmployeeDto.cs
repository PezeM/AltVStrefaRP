using AltV.Net;

namespace AltVStrefaRPServer.Models.Dto.Fractions
{
    public class FractionEmployeeDto : IMValueConvertible
    {
        private static readonly FractionEmployeeAdapter _myAdapter = new FractionEmployeeAdapter();
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int RankId {get; set; }
        public string RankName { get; set; }

        public IMValueBaseAdapter GetAdapter() => _myAdapter;
    }

    public class FractionEmployeeAdapter : IMValueAdapter<FractionEmployeeDto>
    {
        public FractionEmployeeDto FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            int id = 0;
            string name = null;
            string lastName = null;
            string rankName = null;
            int age = 0;
            int rankId = 0;
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "id":
                        id = reader.NextInt();
                        break;
                    case "age":
                        age = reader.NextInt();
                        break;
                    case "ranId":
                        rankId = reader.NextInt();
                        break;
                    case "name":
                        name = reader.NextString();
                        break;
                    case "lastName":
                        lastName = reader.NextString();
                        break;
                    case "rankName":
                        rankName = reader.NextString();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return id == 0
                ? null
                : new FractionEmployeeDto
                {
                    Id = id,
                    Name = name,
                    RankName = rankName,
                    LastName = lastName,
                    Age = age,
                    RankId = rankId
                };
        }

        public void ToMValue(FractionEmployeeDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("name");
            writer.Value(value.Name);
            writer.Name("lastName");
            writer.Value(value.LastName);
            writer.Name("rankName");
            writer.Value(value.RankName);
            writer.Name("age");
            writer.Value(value.Age);
            writer.Name("rankId");
            writer.Value(value.RankId);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer)
        {
            if (obj is FractionEmployeeDto value)
            {
                ToMValue(value, writer);
            }
        }

        object IMValueBaseAdapter.FromMValue(IMValueReader reader) => FromMValue(reader);
    }
}
