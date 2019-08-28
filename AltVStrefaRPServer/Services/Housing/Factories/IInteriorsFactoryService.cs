using System.Collections.Generic;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public interface IInteriorsFactoryService
    {
        IEnumerable<Interior> CreateDefaultInteriors();
    }
}