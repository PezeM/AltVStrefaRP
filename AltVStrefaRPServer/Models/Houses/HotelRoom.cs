using AltVStrefaRPServer.Models.Houses.Interfaces;

namespace AltVStrefaRPServer.Models.Houses
{
    public class HotelRoom : IHotelRoom
    {
        public int Id { get; private set; }
        public Character Owner { get; private set; }
        public int? OwnerId { get; private set; }
        public Interior Interior { get; set; }
        public int InteriorId { get; private set; }
        public Hotel Hotel { get; private set; }
        public int HotelId { get; private set; }

        public int HotelRoomNumber { get; set; }
        public string LockPattern { get; private set; }
        public bool IsLocked { get; private set; }

        public bool HasOwner()
        {
            throw new System.NotImplementedException();
        }

        public bool ChangeOwner(Character newOwner)
        {
            throw new System.NotImplementedException();
        }

        public bool MovePlayerInside(IStrefaPlayer player)
        {
            throw new System.NotImplementedException();
        }

        public bool MovePlayerOutside(IStrefaPlayer player)
        {
            throw new System.NotImplementedException();
        }

        public void LockDoors()
        {
            throw new System.NotImplementedException();
        }

        public void UnlockDoors()
        {
            throw new System.NotImplementedException();
        }

        public void CreateLockPattern()
        {
            throw new System.NotImplementedException();
        }
    }
}