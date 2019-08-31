using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Inventory;

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
        public bool IsLocked { get; set; } = true;
        
        public HouseBuilding HouseBuilding { get; set; }
        public int HouseBuildingId { get; set; }
        
        public bool HasOwner() => Owner != null;
        
        public string ChangeLockPattern()
        {
            CreateLockPattern();
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
            player.HouseId = HouseBuilding.Id;
        }

        public void MovePlayerOutside(IStrefaPlayer player)
        {
            if (IsLocked) return;
            player.Dimension = 0;
            player.Position = HouseBuilding.GetPosition();
            player.HouseId = 0;
        }
        
        public void ToggleLock()
        {
            IsLocked = !IsLocked;
        }
                
        public void CreateLockPattern()
        {
            LockPattern = AdvancedIdGenerator.Instance.Next;
        }
    }
}