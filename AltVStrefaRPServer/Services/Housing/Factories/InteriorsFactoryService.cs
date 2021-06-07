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
                CreateNewInterior(new Position(266.241f, -1007.3f, -102.008f), new Position(265.150f, -1002.9728f, -99f),  
                    "Małe mieszkanie"),
                CreateNewInterior(new Position(346.49f, -1012.84f, -100.196f), new Position(346.453f, -1002.456f, -99.19622f),  
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