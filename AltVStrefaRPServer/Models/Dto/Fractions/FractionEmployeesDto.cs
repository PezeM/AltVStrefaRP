using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Elements.Args;

namespace AltVStrefaRPServer.Models.Dto.Fractions
{
    public class FractionEmployeesDto : IMValueConvertible
    {
        private static readonly IMValueBaseAdapter _myAdapter = new FractionEmployeesDtoAdapter();
        public List<FractionEmployeeDto> Employees { get; set; }
        public List<FractionRankDto> Ranks { get; set; }

        public IMValueBaseAdapter GetAdapter() => _myAdapter;
    }

    public class FractionEmployeesDtoAdapter : IMValueAdapter<FractionEmployeesDto>
    {
        private readonly IMValueAdapter<List<FractionEmployeeDto>> _fractionEmployeeAdapter;
        private readonly IMValueAdapter<List<FractionRankDto>> _fractionRankAdapter;

        public FractionEmployeesDtoAdapter()
        {
            _fractionEmployeeAdapter = DefaultMValueAdapters.GetArrayAdapter(new FractionEmployeeAdapter());
            _fractionRankAdapter = DefaultMValueAdapters.GetArrayAdapter(new FractionRankAdapter());
        }

        public FractionEmployeesDto FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            List<FractionEmployeeDto> employees = null;
            List<FractionRankDto> ranks = null;
            while (reader.HasNext())
            { 
                switch (reader.NextName())
                {
                    case "employees":
                        employees = _fractionEmployeeAdapter.FromMValue(reader);
                        break;
                    case "ranks":
                        ranks = _fractionRankAdapter.FromMValue(reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return employees == null
                ? null
                : new FractionEmployeesDto
                {
                    Employees = employees,
                    Ranks = ranks
                };
        }

        public void ToMValue(FractionEmployeesDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("employees");
            _fractionEmployeeAdapter.ToMValue(value.Employees, writer);
            writer.Name("ranks");
            _fractionRankAdapter.ToMValue(value.Ranks, writer);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer) 
        { 
            if (obj is FractionEmployeesDto value) 
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
