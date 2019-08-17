using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IBusinessesManager : IManager<Business>
    {
        Business GetBusiness(Character employee);
        Business GetBusiness(int businessId);
        bool TryGetBusiness(int businessId, out Business business);
        List<Business> GetCharacterBusinesses(int ownerId);
        Business GetNearestBusiness(IPlayer player);
        Task<bool> AddNewBusinessRankAsync(Business business, BusinessNewRankDto newRank);
        Task<bool> AddNewEmployeeAsync(Business business, Character newEmployee);
        bool CheckIfBusinessExists(string businessName);
        Task<bool> CreateNewBusinessAsync(int ownerId, BusinessType businessType, Position position, string name);
        Task<bool> DeleteBusinessAsync(Business business);
        Task<bool> RemoveEmployeeAsync(Business business, Character employee);
        Task<bool> RemoveRankAsync(Business business, int rankId);
        bool TryGetBusiness(Character employee, out Business business);
        Task<bool> UpdateBusinessOwnerAsync(Business business, Character newOwner);
        Task UpdateBusinessRankAsync(BusinessRank businessRankToUpdate, BusinessPermissionsDto newPermissions);
        Task UpdateEmployeeRankAsync(Business business, Character employee, int newRankId);
    }
}