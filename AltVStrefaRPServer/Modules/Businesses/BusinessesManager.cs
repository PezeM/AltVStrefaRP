using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Extensions;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services.Businesses;
using AltVStrefaRPServer.Services.Characters;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.Businesses
{
    public class BusinessesManager : IBusinessesManager
    {
        private Dictionary<int, Business> _businesses;

        private readonly IBusinessService _businessService;
        private readonly IBusinessDatabaseService _businessDatabaseService;
        private readonly ICharacterDatabaseService _characterDatabaseService;
        private readonly BusinessFactory _businessFactory;
        private readonly ILogger<BusinessesManager> _logger;

        public BusinessesManager(IBusinessService businessService, IBusinessDatabaseService businessDatabaseService, ICharacterDatabaseService characterDatabaseService, 
            ILogger<BusinessesManager> logger)
        {
            _businesses = new Dictionary<int, Business>();

            _characterDatabaseService = characterDatabaseService;
            _businessDatabaseService = businessDatabaseService;
            _businessService = businessService;
            _businessFactory = new BusinessFactory();
            _logger = logger;

            LoadBusinesses();
        }

        /// <summary>
        /// Get business by it's id
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public Business GetBusiness(int businessId) => _businesses.ContainsKey(businessId) ? _businesses[businessId] : null;

        public Business GetBusiness(Character employee)
            => employee.CurrentBusinessId.HasValue ? GetBusiness(employee.CurrentBusinessId.Value) : null;

        public bool TryGetBusiness(Character employee, out Business business)
            => _businesses.TryGetValue(employee.CurrentBusinessId.GetValueOrDefault(), out business);

        /// <summary>
        /// Gets all owned businesses for given character id
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public List<Business> GetCharacterBusinesses(int ownerId) => _businesses.Values.Where(b => b.OwnerId == ownerId).ToList();

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
        public bool CheckIfBusinessExists(string businessName) => _businesses.Values.Any(b => b.BusinessName == businessName);

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
            if (businessType == BusinessType.None || name.IsNullOrEmpty()) return false;
            if (CheckIfBusinessExists(name)) return false;

            var business = _businessFactory.CreateNewBusiness(ownerId, businessType, position, name);
            await _businessDatabaseService.AddNewBusinessAsync(business).ConfigureAwait(false);
            _businesses.Add(business.Id, business);
            _logger.LogInformation("Character ID({characterId}) created new business {@business} in {elapsedTime} ms", ownerId, business, Time.GetTimestampMs() - startTime);
            return true;
        }

        public async Task<bool> UpdateBusinessOwnerAsync(Business business, Character newOwner)
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

        public async Task<bool> AddNewEmployeeAsync(Business business, Character newEmployee)
        {
            if (!_businessService.AddEmployee(business, newEmployee)) return false;
            await _businessDatabaseService.UpdateBusinessAsync(business).ConfigureAwait(false);
            await _characterDatabaseService.UpdateCharacterAsync(newEmployee).ConfigureAwait(false);
            return true;
        }

        public async Task UpdateEmployeeRankAsync(Business business, Character employee, int newRankId)
        {
            employee.BusinessRank = newRankId;
            await _businessDatabaseService.UpdateBusinessAsync(business).ConfigureAwait(false);
            await _characterDatabaseService.UpdateCharacterAsync(employee).ConfigureAwait(false);
        }

        public async Task UpdateBusinessRankAsync(BusinessRank businessRankToUpdate, BusinessPermissionsDto newPermissions)
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

        public async Task<bool> AddNewBusinessRankAsync(Business business, BusinessNewRankDto newRank)
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

        public async Task<bool> RemoveEmployeeAsync(Business business, Character employee)
        {
            if (!business.RemoveEmployee(employee)) return false;
            employee.BusinessRank = 0;

            await Task.WhenAll(_businessDatabaseService.UpdateBusinessAsync(business), _characterDatabaseService.UpdateCharacterAsync(employee));
            return true;
        }

        /// <summary>
        /// Removes rank from business, sets default rank for every employee that had this rank and updates business/characters to database
        /// </summary>
        /// <param name="business"></param>
        /// <param name="rankId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveRankAsync(Business business, int rankId)
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

        public async Task<bool> DeleteBusinessAsync(Business business)
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

        private void LoadBusinesses()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var business in _businessDatabaseService.GetAllBusinesses())
            {
                //_businesses.TryAdd(business.Id, _businessFactory.CreateNewBusiness(business));
                _businesses.TryAdd(business.Id, business);
                //_businessFactory.CreateBusiness(business);
            }
            _logger.LogInformation("Loaded {businessesCount} businesses from databse in {elapsedTime} ms", _businesses.Count, Time.GetTimestampMs() - startTime);
        }
    }
}
