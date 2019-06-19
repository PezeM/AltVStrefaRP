using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Elements.Args;

namespace AltVStrefaRPServer.Models.Dto.Fractions
{
    public class FractionResidentDto : IWritable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int BankAccount { get; set; } = 0;
        public float BankMoney { get; set; } = 0;
        public string FractionName { get; set; } = "Brak";
        public string BusinessName { get; set; } = "Brak";
        //public List<VehicleDataDto> Vehicles { get; set; } = new List<VehicleDataDto>();

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("id");
            writer.Value(Id);
            writer.Name("name");
            writer.Value(Name);
            writer.Name("lastName");
            writer.Value(LastName);
            writer.Name("age");
            writer.Value(Age);
            writer.Name("bankAccount");
            writer.Value(BankAccount);
            writer.Name("bankMoney");
            writer.Value(BankMoney);
            writer.Name("fractionName");
            writer.Value(FractionName);
            writer.Name("businessName");
            writer.Value(BusinessName);
            writer.EndObject();
        }
    }

    public class VehicleDataDto : IMValueConvertible
    {
        private class VehicleDataAdapter : IMValueAdapter<VehicleDataDto>
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

        //public void OnWrite(IMValueWriter writer)
        //{
        //    writer.BeginObject();
        //    writer.Name("model");
        //    writer.Value(Model);
        //    writer.Name("plateText");
        //    writer.Value(PlateText);
        //    writer.EndObject();
        //}
    }

    public class ConvertibleObject : IMValueConvertible
    {
        private class ConvertibleObjectAdapter : IMValueAdapter<ConvertibleObject>
        {
            private readonly IMValueAdapter<List<ConvertibleObject>> listAdapter;

            public ConvertibleObjectAdapter()
            {
                listAdapter = DefaultMValueAdapters.GetArrayAdapter(this);
            }

            public ConvertibleObject FromMValue(IMValueReader reader)
            {
                reader.BeginObject();
                string test = null;
                List<ConvertibleObject> list = null;
                while (reader.HasNext())
                {
                    switch (reader.NextName())
                    {
                        case "test":
                            test = reader.NextString();
                            break;
                        case "list":
                            list = listAdapter.FromMValue(reader);
                            break;
                        default:
                            reader.SkipValue();
                            break;
                    }
                }

                reader.EndObject();
                return test == null ? null : new ConvertibleObject(test, list);
            }

            public void ToMValue(ConvertibleObject value, IMValueWriter writer)
            {
                writer.BeginObject();
                writer.Name("test");
                writer.Value(value.test);
                listAdapter.ToMValue(value, writer);
                writer.EndObject();
            }

            object IMValueBaseAdapter.FromMValue(IMValueReader reader)
            {
                return FromMValue(reader);
            }

            public void ToMValue(object obj, IMValueWriter writer)
            {
                if (obj is ConvertibleObject value)
                {
                    ToMValue(value, writer);
                }
            }
        }

        private static readonly IMValueBaseAdapter MyAdapter = new ConvertibleObjectAdapter();

        private readonly string test;
        private readonly List<ConvertibleObject> list;

        public ConvertibleObject()
        {
            test = "123";
            list = new List<ConvertibleObject>();
        }

        private ConvertibleObject(string test, List<ConvertibleObject> list)
        {
            this.test = test;
            this.list = list;
        }

        public IMValueBaseAdapter GetAdapter()
        {
            return MyAdapter;
        }
    }
}
