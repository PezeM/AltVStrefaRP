using System.Collections.Generic;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AltVStrefaRPServer.Services.Businesses
{
    public class BusinessService : IBusinessService
    {
        private ServerContext _serverContext;

        public BusinessService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        /// <summary>
        /// Gets all business from databse and load everything with eager loading
        /// </summary>
        /// <returns></returns>
        public List<Business> GetAllBusinesses() 
            => _serverContext.Businesses.Include(b => b.Employees)
                .Include(b => b.BusinessRanks)
                .ThenInclude(r => r.Permissions)
                .ToList();

        /// <summary>
        /// Updates business owner and saves changes to database
        /// </summary>
        /// <param name="business">The business to update</param>
        /// <param name="newOwner">New owner of the business</param>
        /// <returns></returns>
        public async Task UpdateOwnerAsync(Business business, Character newOwner)
        {
            business.OwnerId = newOwner.Id;
            _serverContext.Characters.Update(newOwner);
            await UpdateBusinessAsync(business).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds new employee to business. Fails if can't add new emplyoyee or there is no default rank set
        /// </summary>
        /// <param name="business"></param>
        /// <param name="newEmployee"></param>
        /// <returns></returns>
        public bool AddEmployee(Business business, Character newEmployee)
        {
            if (!business.CanAddNewMember(newEmployee)) return false;
            if (!business.SetDefaultRank(newEmployee)) return false;
            business.AddNewMember(newEmployee);
            return true;
        }

        /// <summary>
        /// Updates business in database
        /// </summary>
        /// <param name="business">The business to save to database</param>
        /// <returns></returns>
        public async Task UpdateBusinessAsync(Business business)
        {
            _serverContext.Businesses.Update(business);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddNewBusinessAsync(Business business)
        {
            await _serverContext.Businesses.AddAsync(business).ConfigureAwait(false);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task AddNewBusiness(Business business)
        {
            _serverContext.Businesses.Add(business);
            _serverContext.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task UpdateBusinessRankAsync(BusinessRank newBusinessPermissions)
        {
            _serverContext.BusinessesRanks.Update(newBusinessPermissions);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
