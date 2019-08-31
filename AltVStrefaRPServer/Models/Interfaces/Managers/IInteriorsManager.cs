using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IInteriorsManager : IManager<Interior>
    {
        bool TryGetInterior(int interiorId, out IInterior interior);
        IInterior GetInterior(int interiorId);
    }
}