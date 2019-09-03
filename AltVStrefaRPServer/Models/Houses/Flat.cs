using AltVStrefaRPServer.Helpers;

namespace AltVStrefaRPServer.Models.Houses
{
    public class Flat
    {
        public int Id { get; set; }
        public Character Owner { get; private set; }
        public int? OwnerId { get; private set; }
        // public InventoryContainer InventoryContainer { get; set; }

        public Interior Interior { get; set; }
        public int InteriorId { get; private set; }
        public string LockPattern { get; private set; }
        public bool IsLocked { get; private set; } = true;
        
        public HouseBuilding HouseBuilding { get; set; }
        public int HouseBuildingId { get; set; }

        public bool HasOwner() => OwnerId > 0;
        
        public bool ChangeOwner(Character newOwner)
        {
            if (newOwner.Id == Owner?.Id) return false;
            Owner = newOwner;
            return true;
        }

        public bool MovePlayerInside(IStrefaPlayer player)
        {
            if(Interior == null || IsLocked) return false; 
            player.Dimension = (short) Id;
            player.Position = Interior.GetEnterPosition();
            player.EnteredFlat = this;
            player.Emit("playerMovedInsideHouse");
            return true;
        }

        public bool MovePlayerOutside(IStrefaPlayer player)
        {
            if (IsLocked) return false;
            player.Dimension = 0;
            player.Position = HouseBuilding.GetPosition();
            player.EnteredFlat = null;
            player.Emit("showInteriorExitMenu", false);
            return true;
        }
        
        public void Lock()
        {
            IsLocked = true;
            if(HouseBuilding == null || !(HouseBuilding is House house)) return;
            if (house.Marker == null) return;
            house.Marker.Red = 200;
        }

        public void Unlock()
        {
            IsLocked = false;
            if (HouseBuilding == null || !(HouseBuilding is House house)) return;
            if (house.Marker == null) return;
            house.Marker.Red = 30;
        }

        public void CreateLockPattern()
        {
            LockPattern = AdvancedIdGenerator.Instance.Next;
        }
        
        public string ChangeLockPattern()
        {
            CreateLockPattern();
            return LockPattern;
        }
    }
}