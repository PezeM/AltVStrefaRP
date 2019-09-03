using System.Drawing;
using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Modules.Core;
using AltVStrefaRPServer.Services.MValueAdapters;

namespace AltVStrefaRPServer.Models.Houses
{
    public class House : HouseBuilding, IWritable
    {
        public Flat Flat { get; set; }
        
        public override void InitializeHouse()
        {
            Colshape = (IStrefaColshape) Alt.CreateColShapeCylinder(GetPosition(), 1f, 1.5f);
            Colshape.HouseId = Id;

            Marker = MarkerManager.Instance.AddMarker(21, GetPosition(), Color.FromArgb(255, Flat == null ? 255 : Flat.IsLocked ? 200 : 30, 40, 100),
                new Position(0.8f, 0.8f, 1f), 10, 0);
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