using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Services.Businesses;
using AltVStrefaRPServer.Services.Characters;

namespace AltVStrefaRPServer.Modules.Businesses
{
    public class BusinessManager
    {
        private IBusinessService _businessService;
        private ICharacterDatabaseService _characterDatabaseService;
        private Dictionary<int, Business> Businesses = new Dictionary<int, Business>();
        private BusinessFactory _businessFactory;

        public BusinessManager(IBusinessService businessService, ICharacterDatabaseService characterDatabaseService)
        {
            _characterDatabaseService = characterDatabaseService;
            _businessService = businessService;
            _businessFactory = new BusinessFactory();

            LoadBusinesses();
        }

        private void LoadBusinesses()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var business in _businessService.GetAllBusinesses())
            {
                //Businesses.TryAdd(business.Id, _businessFactory.CreateNewBusiness(business));
                Businesses.TryAdd(business.Id, business);
                //_businessFactory.CreateNewBusiness(business);
            }
            Alt.Log($"Loaded {Businesses.Count} businesses from database in {Time.GetTimestampMs() - startTime}ms.");
        }

        /// <summary>
        /// Get business by it's id
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public Business GetBusiness(int businessId) => Businesses.ContainsKey(businessId) ? Businesses[businessId] : null;

        /// <summary>
        /// Gets all owned businesses for given character id
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public List<Business> GetCharacterBusinesses(int ownerId) => Businesses.Values.Where(b => b.OwnerId == ownerId).ToList();

        public Business GetBusiness(Character emplyoee)
        {
            return Businesses.Values.FirstOrDefault(b => b.Employees.Any(c => c.Id == emplyoee.Id));
        }

        /// <summary>
        /// Get nearest business to player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Business GetNearestBusiness(IPlayer player)
        {
            Business nearestBusiness = null;
            foreach (var business in Businesses.Values)
            {
                if (nearestBusiness == null)
                {
                    nearestBusiness = business;
                    break;
                }
                else
                {
                    if (business.GetPosition().Distance(player.Position) < nearestBusiness.GetPosition().Distance(player.Position))
                    {
                        nearestBusiness = business;
                    }
                }
            }
            return nearestBusiness;
        }

        /// <summary>
        /// Check if business with given name is already registered
        /// </summary>
        /// <param name="businessName"></param>
        /// <returns></returns>
        public bool CheckIfBusinessExists(string businessName)
        {
            return Businesses.Values.Any(b => b.BusinessName == businessName);
        }

        /// <summary>
        /// Create new business and save it to database
        /// </summary>
        /// <param name="businessType">Type of the business <see cref="BusinessType"/></param>
        /// <param name="position">Position where the business will be located</param>
        /// <param name="name">Name of the business</param>
        /// <returns></returns>
        public async Task<bool> CreateNewBusinessAsync(int ownerId, BusinessType businessType, Position position, string name)
        {
            var startTime = Time.GetTimestampMs();
            if(businessType == BusinessType.None || name.IsNullOrEmpty()) return false;
            if (CheckIfBusinessExists(name)) return false;

            var business = _businessFactory.CreateNewBusiness(ownerId, businessType, position, name);
            await _businessService.AddNewBusinessAsync(business).ConfigureAwait(false);
            Businesses.Add(business.Id, business);
            Alt.Log($"Created new business ID({business.Id}) Name({business.BusinessName}) in {Time.GetTimestampMs() - startTime}ms.");
            return true;
        }

        public async Task<bool> UpdateBusinessOwner(Business business, Character newOwner)
        {
            if (newOwner.Business != business)
            {
                if (!_businessService.AddEmployee(business, newOwner)) return false;
                newOwner.BusinessRank = business.BusinessRanks.FirstOrDefault(r => r.IsOwnerRank).Id;
                await _businessService.UpdateOwnerAsync(business, newOwner).ConfigureAwait(false);
                return true;
            }
            else if (newOwner.Business == business)
            {
                newOwner.BusinessRank = business.BusinessRanks.FirstOrDefault(r => r.IsOwnerRank).Id;
                await _businessService.UpdateOwnerAsync(business, newOwner).ConfigureAwait(false);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AddNewEmployee(Business business, Character newEmployee)
        {
            if (!_businessService.AddEmployee(business, newEmployee)) return false;
            await _businessService.UpdateBusinessAsync(business).ConfigureAwait(false);
            await _characterDatabaseService.SaveCharacterAsync(newEmployee).ConfigureAwait(false);
            return true;
        }

        public async Task UpdateEmployeeRank(Business business, Character employee, int newRankId)
        {
            employee.BusinessRank = newRankId;
            await _businessService.UpdateBusinessAsync(business).ConfigureAwait(false);
            await _characterDatabaseService.SaveCharacterAsync(employee).ConfigureAwait(false);
        }

        public async Task UpdateBusinessRank(BusinessRank businessRankToUpdate, BusinessPermissionsDto newPermissions)
        {
            businessRankToUpdate.Permissions.CanInviteNewMembers = newPermissions.CanInviteNewMembers;
            businessRankToUpdate.Permissions.CanManageEmployess = newPermissions.CanManageEmployees;
            businessRankToUpdate.Permissions.CanManageRanks = newPermissions.CanManageRanks;
            businessRankToUpdate.Permissions.CanOpenBusinessInventory = newPermissions.CanOpenBusinessInventory;
            businessRankToUpdate.Permissions.CanOpenBusinessMenu = newPermissions.CanOpenBusinessMenu;
            businessRankToUpdate.Permissions.HaveBusinessKeys = newPermissions.HaveBusinessKeys;
            businessRankToUpdate.Permissions.HaveVehicleKeys = newPermissions.HaveVehicleKeys;

            await _businessService.UpdateBusinessRankAsync(businessRankToUpdate);
        }

        public async Task<bool> AddNewBusinessRank(Business business, BusinessNewRankDto newRank)
        {
            if (!business.CanAddNewRank()) return false;
            business.BusinessRanks.Add(new BusinessRank
            {
                RankName = newRank.RankName,
                IsDefaultRank = false,
                IsOwnerRank = false,
                Permissions = new BusinessPermissions
                {
                    CanInviteNewMembers = newRank.Permissions.CanInviteNewMembers,
                    CanManageEmployess = newRank.Permissions.CanManageEmployess,
                    CanOpenBusinessInventory = newRank.Permissions.CanOpenBusinessInventory,
                    CanOpenBusinessMenu = newRank.Permissions.CanOpenBusinessMenu,
                    CanManageRanks = newRank.Permissions.CanManageRanks,
                    HaveVehicleKeys = newRank.Permissions.HaveVehicleKeys,
                    HaveBusinessKeys = newRank.Permissions.HaveBusinessKeys,
                }
            });
            await _businessService.UpdateBusinessAsync(business).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> RemoveEmployee(Business business, Character employee)
        {
            if (!business.RemoveEmployee(employee)) return false;
            employee.BusinessRank = -1;

            await Task.WhenAll(_businessService.UpdateBusinessAsync(business), _characterDatabaseService.SaveCharacterAsync(employee));
            return true;
        }
    }
}
