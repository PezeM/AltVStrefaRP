using AltV.Net;

namespace AltVStrefaRPServer.Models.Dto.Fractions
{
    public class TownHallFractionDto : IMValueConvertible
    {
        private static readonly IMValueBaseAdapter _myAdapter = new FractionResidentDtoAdapter();
        public int id {get;set;}
        public float money {get;set;}
        public int employeesCount {get; set;}
        public int rolesCount {get;set;}
        public string creationDate {get;set;}
        public float vehicleTax {get;set;}
        public float propertyTax {get;set;}
        public float gunTax {get;set;}
        public float globalTax {get;set;}

        public IMValueBaseAdapter GetAdapter()
        {
            return _myAdapter;
        }
    }

    public class TownHallFractionAdapter : IMValueAdapter<TownHallFractionDto>
    {
        public TownHallFractionDto FromMValue(IMValueReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void ToMValue(TownHallFractionDto value, IMValueWriter writer)
        {
            //writer.BeginObject();
            //writer.Name("id");
            //writer.Value(value.id);
            //writer.Name("name");
            //writer.Value(value.Name);
            //writer.Name("lastName");
            //writer.Value(value.LastName);
            //writer.Name("age");
            //writer.Value(value.Age);
            //writer.Name("bankAccount");
            //writer.Value(value.BankAccount);
            //writer.Name("bankMoney");
            //writer.Value(value.BankMoney);
            //writer.Name("fractionName");
            //writer.Value(value.FractionName);
            //writer.Name("businessName");
            //writer.Value(value.BusinessName);
            //writer.Name("vehicles");
            //_vehicleListAdapter.ToMValue(value, writer);
            //writer.EndObject();
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
