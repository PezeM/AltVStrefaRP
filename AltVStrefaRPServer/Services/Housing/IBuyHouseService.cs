using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing
{
    public interface IBuyHouseService
    {
        Task<BuyHouseResponse> BuyHouseAsync(Character newOwner, OldHouse oldHouse);
    }
}