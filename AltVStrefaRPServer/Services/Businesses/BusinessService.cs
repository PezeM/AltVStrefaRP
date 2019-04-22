using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using System.Threading.Tasks;

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
        /// Updates business owner and saves changes to database
        /// </summary>
        /// <param name="business">The business to update</param>
        /// <param name="newOwner">New owner of the business</param>
        /// <returns></returns>
        public async Task UpdateOwnerAsync(Business business,Character newOwner)
        {
            business.OwnerId = newOwner.Id;
            await UpdateAsync(business).ConfigureAwait(false);
        }

        public bool AddEmployee(Business business, Character newEmployee)
        {
            if (!business.CanAddNewMember(newEmployee)) return false;
            business.AddNewMember(newEmployee);
            if (!business.SetDefaultRank(newEmployee)) return false;
            return true;
        }

        /// <summary>
        /// Updates business in database
        /// </summary>
        /// <param name="business">The business to save to database</param>
        /// <returns></returns>
        public async Task UpdateAsync(Business business)
        {
            _serverContext.Businesses.Update(business);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddNewBusinessAsync(Business business)
        {
            await _serverContext.Businesses.AddAsync(business).ConfigureAwait(false);
            await _serverContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
