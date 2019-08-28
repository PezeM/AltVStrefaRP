using System;
using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Core;
using AltVStrefaRPServer.Models.Interfaces;
using AltVStrefaRPServer.Services.MValueAdapters;

namespace AltVStrefaRPServer.Models.Houses
{
    public class House : IPosition, IWritable
    {
        public int Id { get; set; }
        public Character Owner { get; private set; }
        public int? OwnerId { get; private set; }
        
        // Don't know if its needed and will do M-1 
        public Interior Interior { get; set; }
        public int InteriorId { get; private set; }
        
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Price { get; set; }
        public bool IsLocked { get; set; }
        public string LockPattern { get; private set; }
        
        public IStrefaColshape Colshape { get; private set; }
        
        public void InitializeHouse()
        {
            Colshape = (IStrefaColshape) Alt.CreateColShapeCylinder(GetPosition(), 1f, 1f);
            Colshape.HouseId = Id;
        }
        
        public Position GetPosition() => new Position(X, Y, Z);
        public bool HasOwner() => Owner != null;
        
        public string ChangeLockPattern()
        {
            LockPattern = new Guid().ToString();
            return LockPattern;
        }

        public bool ChangeOwner(Character newOwner)
        {
            if (newOwner.Id == Owner?.Id || Owner != null) return false;
            Owner = newOwner;
            return true;
        }

        public void MovePlayerInside(IStrefaPlayer player)
        {
            if(Interior == null) return;
            player.Dimension = (short) Id;
            player.Position = Interior.GetPosition();
            player.HouseId = Id;
        }

        public void MovePlayerOutside(IStrefaPlayer player)
        {
            if (IsLocked) return;
            player.Dimension = 0;
            player.Position = GetPosition();
            player.HouseId = 0;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("price");
            writer.Value(Price);
            writer.Name("owner");
            writer.Value(HasOwner());
            writer.Name("isClosed");
            writer.Value(IsLocked);
            writer.Name("interiorName");
            writer.Value(Interior?.Name);
            writer.Name("position");
            new PositionAdapter().ToMValue(GetPosition(), writer);
            writer.EndObject();
        }
    }
}
