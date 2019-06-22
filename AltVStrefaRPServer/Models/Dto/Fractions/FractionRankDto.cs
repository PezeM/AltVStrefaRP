using AltV.Net;

namespace AltVStrefaRPServer.Models.Dto.Fractions
{
    public class FractionRankDto : IMValueConvertible
    {
        private static readonly FractionRankAdapter _mAdapter = new FractionRankAdapter();
        public int Id { get; set; }
        public bool IsDefaultRank { get; set; }
        public bool IsHighestRank { get; set; }
        public string RankName { get; set; }

        public IMValueBaseAdapter GetAdapter() => _mAdapter;
    }

    public class FractionRankAdapter : IMValueAdapter<FractionRankDto>
    {
        public FractionRankDto FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            int id = 0;
            string rankName = null;
            bool isDefaultRank = false;
            bool isHighestRank = false;
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
                    case "isDefaultRank":
                        isDefaultRank = reader.NextBool();
                        break;
                    case "isHighestRank":
                        isHighestRank = reader.NextBool();
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
                    IsDefaultRank = isDefaultRank,
                    IsHighestRank = isHighestRank
                };
        }

        public void ToMValue(FractionRankDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("rankName");
            writer.Value(value.RankName);
            writer.Name("isDefaultRank");
            writer.Value(value.IsDefaultRank);
            writer.Name("isHighestRank");
            writer.Value(value.IsHighestRank);
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
