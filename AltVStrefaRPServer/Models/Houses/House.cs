using System.Drawing;
using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Models.Houses.Interfaces;
using AltVStrefaRPServer.Modules.Core;
using AltVStrefaRPServer.Services.MValueAdapters;

namespace AltVStrefaRPServer.Models.Houses
{
    public class House : IHouseBuilding, IHouse
    {
        public int Id { get; private set; }
        public Character Owner { get; private set; }
        public int? OwnerId { get; private set; }
        public Interior Interior { get; set; }
        public int InteriorId { get; private set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public int Price { get; set; }
        public string LockPattern { get; private set; }
        public bool IsLocked { get; private set; } = true;

        public IStrefaColshape Colshape { get; protected set; }
        public IMarker Marker { get; protected set; }

        public Position GetPosition() => new Position(X, Y, Z);

        public bool HasOwner() => OwnerId > 0;

        public bool ChangeOwner(Character newOwner)
        {
            if (OwnerId == newOwner.Id) return false;
            Owner = newOwner;
            return true;
        }

        public bool MovePlayerInside(IStrefaPlayer player)
        {
            if (Interior == null || IsLocked) return false;
            player.Dimension = (short)Id;
            player.Position = Interior.GetEnterPosition();
            player.EnteredHouse = this;
            player.Emit("playerMovedInsideHouse");
            return true;
        }

        public bool MovePlayerOutside(IStrefaPlayer player)
        {
            if (IsLocked) return false;
            player.Dimension = 0;
            player.Position = GetPosition();
            player.EnteredHouse = null;
            player.Emit("showInteriorExitMenu", false);
            return true;
        }

        public void LockDoors()
        {
            IsLocked = true;
            if (Marker == null) return;
            Marker.Red = 200;
        }

        public void UnlockDoors()
        {
            IsLocked = false;
            if (Marker == null) return;
            Marker.Red = 30;
        }

        public void CreateLockPattern()
        {
            LockPattern = AdvancedIdGenerator.Instance.Next;
        }

        public void InitializeHouseBuilding()
        {
            Colshape = (IStrefaColshape) Alt.CreateColShapeCylinder(GetPosition(), 1f, 1.5f);
            Colshape.HouseId = Id;

            Marker = MarkerManager.Instance.AddMarker(21, GetPosition(), Color.FromArgb(255, IsLocked ? 200 : 30, 40, 100),
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
            writer.Value(HasOwner());
            writer.Name("isClosed");
            writer.Value(IsLocked);
            writer.Name("interiorName");
            writer.Value(Interior?.Name);
            writer.Name("position");
            Adapters.PositionAdatper.ToMValue(GetPosition(), writer);
            writer.EndObject();
        }
    }
}