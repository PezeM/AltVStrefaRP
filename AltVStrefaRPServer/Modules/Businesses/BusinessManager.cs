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
        private Dictionary<int, Business> _businesses;

        private IBusinessService _businessService;
        private IBusinessDatabaseService _businessDatabaseService;
        private ICharacterDatabaseService _characterDatabaseService;
        private BusinessFactory _businessFactory;

        public BusinessManager(IBusinessService businessService, IBusinessDatabaseService businessDatabaseService, ICharacterDatabaseService characterDatabaseService)
        {
            _characterDatabaseService = characterDatabaseService;
            _businessDatabaseService = businessDatabaseService;
            _businessService = businessService;
            _businessFactory = new BusinessFactory();

            _businesses = new Dictionary<int, Business>();
            LoadBusinesses();
        }

        private void LoadBusinesses()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var business in _businessDatabaseService.GetAllBusinesses())
            {
                //_businesses.TryAdd(business.Id, _businessFactory.CreateNewBusiness(business));
                _businesses.TryAdd(business.Id, business);
                //_businessFactory.CreateBusiness(business);
            }
            Alt.Log($"Loaded {_businesses.Count} businesses from database in {Time.GetTimestampMs() - startTime}ms.");
        }

        /// <summary>
        /// Get business by it's id
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public Business GetBusiness(int businessId)
        {
            return _businesses.ContainsKey(businessId) ? _businesses[businessId] : null;
        }

        /// <summary>
        /// Gets all owned businesses for given character id
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public List<Business> GetCharacterBusinesses(int ownerId) => _businesses.Values.Where(b => b.OwnerId == ownerId).ToList();

        public Business GetBusiness(Character employee) 
            => employee.CurrentBusinessId.HasValue ? GetBusiness(employee.CurrentBusinessId.Value) : null;

        public bool TryGetBusiness(Character employee, out Business business) 
            => _businesses.TryGetValue(employee.CurrentBusinessId.GetValueOrDefault(), out business);

        /// <summary>
        /// Get nearest business to player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Business GetNearestBusiness(IPlayer player)
        {
            Business nearestBusiness = null;
            foreach (var business in _businesses.Values)
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
            return _businesses.Values.Any(b => b.BusinessName == businessName);
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
            await _businessDatabaseService.AddNewBusinessAsync(business).ConfigureAwait(false);
            _businesses.Add(business.Id, business);
            Alt.Log($"Created new business ID({business.Id}) Name({business.BusinessName}) in {Time.GetTimestampMs() - startTime}ms.");
            return true;
        }

        public async Task<bool> UpdateBusinessOwner(Business business, Character newOwner)
        {
            if (newOwner.CurrentBusinessId != business.Id)
            {
                if (!_businessService.AddEmployee(business, newOwner)) return false;
                newOwner.BusinessRank = business.BusinessRanks.FirstOrDefault(r => r.IsOwnerRank).Id;
                await _businessDatabaseService.UpdateOwnerAsync(business, newOwner).ConfigureAwait(false);
                return true;
            }
            else if (newOwner.CurrentBusinessId == business.Id)
            {
                newOwner.BusinessRank = business.BusinessRanks.FirstOrDefault(r => r.IsOwnerRank).Id;
                await _businessDatabaseService.UpdateOwnerAsync(business, newOwner).ConfigureAwait(false);
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
            await _businessDatabaseService.UpdateBusinessAsync(business).ConfigureAwait(false);
            await _characterDatabaseService.UpdateCharacterAsync(newEmployee).ConfigureAwait(false);
            return true;
        }

        public async Task UpdateEmployeeRank(Business business, Character employee, int newRankId)
        {
            employee.BusinessRank = newRankId;
            await _businessDatabaseService.UpdateBusinessAsync(business).ConfigureAwait(false);
            await _characterDatabaseService.UpdateCharacterAsync(employee).ConfigureAwait(false);
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

            await _businessDatabaseService.UpdateBusinessRankAsync(businessRankToUpdate);
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
            await _businessDatabaseService.UpdateBusinessAsync(business).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> RemoveEmployee(Business business, Character employee)
        {
            if (!business.RemoveEmployee(employee)) return false;
            employee.BusinessRank = -1;

            await Task.WhenAll(_businessDatabaseService.UpdateBusinessAsync(business), _characterDatabaseService.UpdateCharacterAsync(employee));
            return true;
        }

        /// <summary>
        /// Removes rank from business, sets default rank for every employee that had this rank and updates business/characters to database
        /// </summary>
        /// <param name="business"></param>
        /// <param name="rankId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveRank(Business business, int rankId)
        {
            if (!business.RemoveRank(rankId)) return false;

            var employeesToChange = business.GetEmployeesWithRank(rankId);
            if (employeesToChange.Count() > 0)
            {
                foreach (var character in employeesToChange)
                {
                    business.SetDefaultRank(character);
                }

                await Task.WhenAll(_businessDatabaseService.UpdateBusinessAsync(business), _characterDatabaseService.UpdateCharactersAsync(employeesToChange));
            }
            else
            {
                await _businessDatabaseService.UpdateBusinessAsync(business);
            }

            return true;
        }

        public async Task<bool> DeleteBusiness(Business business)
        {
            foreach (var employee in business.Employees)
            {
                employee.BusinessRank = -1;
            }

            business.Employees.Clear();
            business.BusinessRanks.Clear();
            _businesses.Remove(business.Id);
            await _businessDatabaseService.RemoveBusinessAsync(business).ConfigureAwait(false);
            return true;
        }
    }
}
