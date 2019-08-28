using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing
{
    public interface IInteriorDatabaseService
    {
        IEnumerable<Interior> GetAllInteriors();
        Task AddNewInteriorAsync(Interior interior);
        void AddNewInterior(Interior interior);
        void AddNewInteriors(IEnumerable<Interior> newInteriors);
        Task SaveInteriorAsync(Interior interior);
        void SaveInterior(Interior interior);
        Task RemoveInteriorAsync(Interior interior);
        void RemoveInterior(Interior interior);
    }
}