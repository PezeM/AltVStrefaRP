using System.Threading.Tasks;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Services.Businesses
{
    public interface IBusinessService
    {
        Task UpdateOwnerAsync(Business business, Character newOwner);
        Task Save(Business business);
    }
}
