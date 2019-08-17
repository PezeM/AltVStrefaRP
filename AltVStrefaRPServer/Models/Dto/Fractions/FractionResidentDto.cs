using AltV.Net;
using AltV.Net.Elements.Args;
using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Dto.Fractions
{
    public class FractionResidentDto : IMValueConvertible
    {
        private static readonly IMValueBaseAdapter _myAdapter = new FractionResidentDtoAdapter();
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int BankAccount { get; set; }
        public float BankMoney { get; set; }
        public string FractionName { get; set; }
        public string BusinessName { get; set; }
        public List<VehicleDataDto> Vehicles { get; set; } = new List<VehicleDataDto>();

        public FractionResidentDto(int id, string name, string lastName, int age, int bankAccount, float bankMoney, string fractionName,
            string businessName, List<VehicleDataDto> vehicles)
        {
            Id = id;
            Name = name;
            LastName = lastName;
            Age = age;
            BankAccount = bankAccount;
            BankMoney = bankMoney;
            FractionName = fractionName;
            BusinessName = businessName;
            Vehicles = vehicles;
        }

        public FractionResidentDto() { }

        public IMValueBaseAdapter GetAdapter()
        {
            return _myAdapter;
        }
    }

    public class FractionResidentDtoAdapter : IMValueAdapter<FractionResidentDto>
    {
        private readonly IMValueAdapter<List<VehicleDataDto>> _vehicleListAdapter;
        public FractionResidentDtoAdapter()
        {
            _vehicleListAdapter = DefaultMValueAdapters.GetArrayAdapter(new VehicleDataAdapter());
        }

        public FractionResidentDto FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            int id = 0;
            string name = null;
            string lastName = null;
            int age = 0;
            int bankAccount = 0;
            float bankMoney = 0;
            string fractionName = "Brak";
            string businessName = "Brak";
            List<VehicleDataDto> vehicles = null;
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
                    case "lastName":
                        lastName = reader.NextString();
                        break;
                    case "age":
                        age = reader.NextInt();
                        break;
                    case "bankAccount":
                        bankAccount = reader.NextInt();
                        break;
                    case "bankMoney":
                        bankMoney = (float)reader.NextDouble();
                        break;
                    case "fractionName":
                        fractionName = reader.NextString();
                        break;
                    case "businessName":
                        businessName = reader.NextString();
                        break;
                    case "vehicles":
                        vehicles = _vehicleListAdapter.FromMValue(reader);
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return id == 0 ? null : new FractionResidentDto(id, name, lastName, age, bankAccount, bankMoney, fractionName, businessName, vehicles);
        }

        public void ToMValue(FractionResidentDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(value.Id);
            writer.Name("name");
            writer.Value(value.Name);
            writer.Name("lastName");
            writer.Value(value.LastName);
            writer.Name("age");
            writer.Value(value.Age);
            writer.Name("bankAccount");
            writer.Value(value.BankAccount);
            writer.Name("bankMoney");
            writer.Value(value.BankMoney);
            writer.Name("fractionName");
            writer.Value(value.FractionName);
            writer.Name("businessName");
            writer.Value(value.BusinessName);
            writer.Name("vehicles");
            _vehicleListAdapter.ToMValue(value.Vehicles, writer);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer)
        {
            if (obj is FractionResidentDto value)
            {
                ToMValue(value, writer);
            }
        }

        object IMValueBaseAdapter.FromMValue(IMValueReader reader)
        {
            return FromMValue(reader);
        }
    }


    public class VehicleDataDto : IMValueConvertible
    {
        private static readonly IMValueBaseAdapter _myAdapter = new VehicleDataAdapter();
        public string Model { get; set; }
        public string PlateText { get; set; }

        public VehicleDataDto(string model, string plateText)
        {
            Model = model;
            PlateText = plateText;
        }

        public IMValueBaseAdapter GetAdapter()
        {
            return _myAdapter;
        }
    }

    public class VehicleDataAdapter : IMValueAdapter<VehicleDataDto>
    {
        public VehicleDataDto FromMValue(IMValueReader reader)
        {
            reader.BeginObject();
            string model = null;
            string plateText = null;
            while (reader.HasNext())
            {
                switch (reader.NextName())
                {
                    case "model":
                        model = reader.NextString();
                        break;
                    case "plateText":
                        plateText = reader.NextString();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            reader.EndObject();
            return model == null ? null : new VehicleDataDto(model, plateText);
        }

        public void ToMValue(VehicleDataDto value, IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("model");
            writer.Value(value.Model);
            writer.Name("plateText");
            writer.Value(value.PlateText);
            writer.EndObject();
        }

        public void ToMValue(object obj, IMValueWriter writer)
        {
            if (obj is VehicleDataDto value)
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
