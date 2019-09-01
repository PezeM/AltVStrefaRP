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
                CreateNewInterior(new Position(265.0858f, -1000.888f, -99.00855f), new Position(65.0858f, -1000.888f, -99.00855f),  
                    "Małe mieszkanie"),
                CreateNewInterior(new Position(346.453f, -1002.456f, -99.19622f), new Position(346.453f, -1002.456f, -99.19622f),  
                    "Większe mieszkanie"),
            };
        }

        public Interior CreateNewInterior(Position position, Position enterPosition, string name)
        {
            return new Interior(name, position.X, position.Y, position.Z, enterPosition.X, enterPosition.Y, enterPosition.Z);
        }

        public async Task<Interior> CreateNewInteriorAsync(Position position, Position enterPosition, string name)
        {
            var newInterior = CreateNewInterior(position, enterPosition, name);
            await _interiorDatabaseService.AddNewInteriorAsync(newInterior);
            return newInterior;
        }
    }
}