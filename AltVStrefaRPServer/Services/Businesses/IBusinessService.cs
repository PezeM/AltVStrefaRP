using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;

namespace AltVStrefaRPServer.Services.Businesses
{
    public interface IBusinessService
    {
        Task UpdateOwnerAsync(Business business, Character newOwner);
        Task Save(Business business);
        Task AddNewBusiness(Business business);
    }
}
