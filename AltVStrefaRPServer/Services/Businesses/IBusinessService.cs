using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;

namespace AltVStrefaRPServer.Services.Businesses
{
    public interface IBusinessService
    {
        List<Business> GetAllBusinesses();
        Task UpdateOwnerAsync(Business business, Character newOwner);
        Task UpdateBusinessAsync(Business business);
        Task AddNewBusinessAsync(Business business);
        Task AddNewBusiness(Business business);
        bool AddEmployee(Business business, Character newEmployee);
        Task UpdateBusinessRankAsync(BusinessRank newBusinessPermissions);
        Task RemoveBusinessAsync(Business business);
    }
}
