using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Houses;
using AltVStrefaRPServer.Models.Houses.Responses;

namespace AltVStrefaRPServer.Services.Housing
{
    public interface IBuyHouseService
    {
        Task<BuyHouseResponse> BuyHouseAsync(Character newOwner, House house);
        Task TestChange(House house);
    }
}