using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Houses.Interfaces
{
    public interface IHotel : IHouseBuilding
    {
        int MaximumNumberOfHotelRooms { get; set; }
        ICollection<HotelRoom> HotelRooms { get; set; }
        bool TryGetHotelRoom(int roomNumber, out HotelRoom hotelRoom);
        bool AddNewHotelRoom(HotelRoom hotelRoom);
    }
}