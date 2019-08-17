using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Services.Businesses
{
    public interface IBusinessDatabaseService
    {
        IEnumerable<Business> GetAllBusinesses();
        Task UpdateOwnerAsync(Business business, Character newOwner);
        Task UpdateBusinessAsync(Business business);
        Task AddNewBusinessAsync(Business business);
        int AddNewBusiness(Business business);
        Task UpdateBusinessRankAsync(BusinessRank newBusinessPermissions);
        Task RemoveBusinessAsync(Business business);
    }
}
