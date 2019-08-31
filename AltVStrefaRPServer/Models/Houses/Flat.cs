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
        public bool IsLocked { get; set; } = true;
        
        public HouseBuilding HouseBuilding { get; set; }
        public int HouseBuildingId { get; set; }
        
        public bool HasOwner() => Owner != null;
        
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
            player.Position = Interior.GetPosition();
            player.EnteredFlat = this;
            return true;
        }

        public void MovePlayerOutside(IStrefaPlayer player)
        {
            if (IsLocked) return;
            player.Dimension = 0;
            player.Position = HouseBuilding.GetPosition();
            player.EnteredFlat = null;
        }
        
        public void ToggleLock()
        {
            IsLocked = !IsLocked;
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