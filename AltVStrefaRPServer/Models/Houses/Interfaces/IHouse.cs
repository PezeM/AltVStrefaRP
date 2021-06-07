namespace AltVStrefaRPServer.Models.Houses.Interfaces
{
    public interface IHouse
    {
        Character Owner { get; }
        int? OwnerId { get; }
        Interior Interior { get; set; }
        int InteriorId { get; }
        string LockPattern { get; }
        bool IsLocked { get; }
        bool HasOwner();

        bool ChangeOwner(Character newOwner);
        bool MovePlayerInside(IStrefaPlayer player);
        bool MovePlayerOutside(IStrefaPlayer player);
        void LockDoors();
        void UnlockDoors();
        void CreateLockPattern();
    }
}
