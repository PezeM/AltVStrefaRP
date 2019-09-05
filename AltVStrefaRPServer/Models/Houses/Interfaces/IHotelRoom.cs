namespace AltVStrefaRPServer.Models.Houses.Interfaces
{
    public interface IHotelRoom : IHouse
    {
        int Id { get; }
        Hotel Hotel { get; }
        int HotelId { get; }
        int HotelRoomNumber { get; set; }
    }
}