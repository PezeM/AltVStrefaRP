using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Elements.Args;

namespace AltVStrefaRPServer.Models.Dto.Fractions
{
    public class TownHallFractionDto : IMValueConvertible
    {
        private static readonly IMValueBaseAdapter _myAdapter = new FractionResidentDtoAdapter();
        public int Id {get;set;}
        public float Money {get;set;}
        public int EmployeesCount {get; set;}
        public int RolesCount {get;set;}
        public string CreationDate {get;set;}
        public List<TaxDto> Taxes { get; set; }

        public IMValueBaseAdapter GetAdapter()
        {
            return _myAdapter;
        }
    }

    public class TownHallFractionAdapter : IMValueAdapter<TownHallFractionDto>
    {
        private readonly IMValueAdapter<List<TaxDto>> _taxListAdapter;
        public TownHallFractionAdapter() 
        { 
            _taxListAdapter = DefaultMValueAdapters.GetArrayAdapter(new TaxDtoAdapter());
        }

        public TownHallFractionDto FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            int id = 0;
            float money = 0;
            int employeesCount = 0;
            int rolesCount = 0;
            string creationDate = null;
            List<TaxDto> taxes = null;
            while (reader.HasNext())
            { 
                switch (reader.NextName())
                {
                    case "id":
                        id = reader.NextInt();
                        break;
                    case "money":
                        money = (float)reader.NextDouble();
                        break;
                    case "employeesCount":
                        employeesCount = reader.NextInt();
                        break;
                    case "rolesCount":
                        rolesCount = reader.NextInt();
                        break;
                    case "creationDate":
                        creationDate = reader.NextString();
                        break;
                    case "taxes":
                        taxes = _taxListAdapter.FromMValue(reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return id == 0
                ? null
                : new TownHallFractionDto
                {
                    Id = id,
                    Money = money,
                    EmployeesCount = employeesCount,
                    CreationDate = creationDate,
                    Taxes = taxes,
                    RolesCount = rolesCount
                };
        }

        public void ToMValue(TownHallFractionDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("money");
            writer.Value(value.Money);
            writer.Name("employeesCount");
            writer.Value(value.EmployeesCount);
            writer.Name("rolesCount");
            writer.Value(value.RolesCount);
            writer.Name("creationDate");
            writer.Value(value.CreationDate);
            writer.Name("taxes");
            _taxListAdapter.ToMValue(value.Taxes, writer);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer)
        {
            if (obj is TownHallFractionDto value)
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
