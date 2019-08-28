using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public class HouseFactoryService : IHouseFactoryService
    {
        public House CreateNewHouse(Position position, int price)
        {
            var newHouse = new House
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                Price = price,
                IsLocked = true,
            };

            newHouse.CreateLockPattern();
            return newHouse;
        }
    }
}