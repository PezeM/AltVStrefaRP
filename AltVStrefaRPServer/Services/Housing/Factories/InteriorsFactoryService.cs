using System.Collections.Generic;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing.Factories
{
    public class InteriorsFactoryService : IInteriorsFactoryService
    {
        public IEnumerable<Interior> CreateDefaultInteriors()
        {
            return new List<Interior>
            {
                new Interior("Małe mieszkanie", 265.0858f, -1000.888f, -99.00855f),
                new Interior("Większe mieszkanie", 346.453f, -1002.456f, -99.19622f)
            };
        }
    }
}