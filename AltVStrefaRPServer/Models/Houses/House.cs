using AltV.Net;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Services.MValueAdapters;

namespace AltVStrefaRPServer.Models.Houses
{
    public class House : HouseBuilding, IWritable
    {
        public Flat Flat { get; set; }
        
        public override void InitializeHouse()
        {
            Colshape = (IStrefaColshape) Alt.CreateColShapeCylinder(GetPosition(), 1f, 1f);
            Colshape.HouseId = Id;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("houseType");
            writer.Value((int)HouseBuildingTypes.House);
            writer.Name("price");
            writer.Value(Price);
            writer.Name("owner");
            writer.Value(Flat.HasOwner());
            writer.Name("isClosed");
            writer.Value(Flat.IsLocked);
            writer.Name("interiorName");
            writer.Value(Flat.Interior?.Name);
            writer.Name("position");
            Adapters.PositionAdatper.ToMValue(GetPosition(), writer);
            writer.EndObject();
        }
    }
}