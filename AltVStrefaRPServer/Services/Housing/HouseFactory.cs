using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing
{
    public class HouseFactory
    {
        public House CreateNewHouse(int interiorId)
        {
            return new House();
        }
    }
}