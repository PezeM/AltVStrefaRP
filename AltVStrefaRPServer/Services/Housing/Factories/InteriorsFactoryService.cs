using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public class InteriorsFactoryService : IInteriorsFactoryService
    {
        private readonly IInteriorDatabaseService _interiorDatabaseService;

        public InteriorsFactoryService(IInteriorDatabaseService interiorDatabaseService)
        {
            _interiorDatabaseService = interiorDatabaseService;
        }
        
        public IEnumerable<Interior> CreateDefaultInteriors()
        {
            return new List<Interior>
            {
                new Interior("Małe mieszkanie", 265.0858f, -1000.888f, -99.00855f),
                new Interior("Większe mieszkanie", 346.453f, -1002.456f, -99.19622f)
            };
        }

        public Interior CreateNewInterior(Position position, string name)
        {
            return new Interior(name, position.X, position.Y, position.Z);
        }

        public async Task<Interior> CreateNewInteriorAsync(Position position, string name)
        {
            var newInterior = CreateNewInterior(position, name);
            await _interiorDatabaseService.AddNewInteriorAsync(newInterior);
            return newInterior;
        }
    }
}