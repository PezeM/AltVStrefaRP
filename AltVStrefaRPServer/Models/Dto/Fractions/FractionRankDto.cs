using AltV.Net;

namespace AltVStrefaRPServer.Models.Dto.Fractions
{
    public class FractionRankDto : IMValueConvertible
    {
        private static readonly FractionRankAdapter _mAdapter = new FractionRankAdapter();
        public int Id { get; set; }
        public string RankName { get; set; }
        public int Priorty { get; set; }
        public int RankType { get; set; }

        public IMValueBaseAdapter GetAdapter() => _mAdapter;
    }

    public class FractionRankAdapter : IMValueAdapter<FractionRankDto>
    {
        public FractionRankDto FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            int id = 0;
            string rankName = null;
            int priority = 0;
            int rankType = 0;
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "id":
                        id = reader.NextInt();
                        break;
                    case "rankName":
                        rankName = reader.NextString();
                        break;
                    case "priority":
                        priority = reader.NextInt();
                        break;
                    case "rankType":
                        rankType = reader.NextInt();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return id == 0
                ? null
                : new FractionRankDto
                {
                    Id = id,
                    RankName = rankName,
                    RankType = rankType,
                    Priorty = priority
                };
        }

        public void ToMValue(FractionRankDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("rankName");
            writer.Value(value.RankName);
            writer.Name("priority");
            writer.Value(value.Priorty);
            writer.Name("rankType");
            writer.Value(value.RankType);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer)
        {
            if (obj is FractionRankDto value)
            {
                ToMValue(value, writer);
            }
        }

        object IMValueBaseAdapter.FromMValue(IMValueReader reader) => FromMValue(reader);
    }
}
