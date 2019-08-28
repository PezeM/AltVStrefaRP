using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public interface IInteriorsFactoryService
    {
        IEnumerable<Interior> CreateDefaultInteriors();
        Interior CreateNewInterior(Position position, string name);
        Task<Interior> CreateNewInteriorAsync(Position position, string name);
    }
}