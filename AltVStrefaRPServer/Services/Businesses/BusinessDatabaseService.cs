using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Businesses
{
    public class BusinessDatabaseService : IBusinessDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public BusinessDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Gets all business from database and load everything with eager loading
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Business> GetAllBusinesses()
        {
            using (var context = _factory.Invoke())
            {
                return context.Businesses
                    .Include(b => b.Employees)
                    .Include(b => b.BusinessRanks)
                    .ThenInclude(r => r.Permissions);
            }
        }

        /// <summary>
        /// Updates business owner and saves changes to database
        /// </summary>
        /// <param name="business">The business to update</param>
        /// <param name="newOwner">New owner of the business</param>
        /// <returns></returns>
        public async Task UpdateOwnerAsync(Business business, Character newOwner)
        {
            using (var context = _factory.Invoke())
            {
                business.OwnerId = newOwner.Id;
                await UpdateBusinessAsync(business);
            }
        }

        /// <summary>
        /// Updates business in database
        /// </summary>
        /// <param name="business">The business to save to database</param>
        /// <returns></returns>
        public Task UpdateBusinessAsync(Business business)
        {
            using (var context = _factory.Invoke())
            {
                context.Businesses.Update(business);
                return context.SaveChangesAsync();
            }
        }

        public async Task AddNewBusinessAsync(Business business)
        {
            using (var context = _factory.Invoke())
            {
                await context.Businesses.AddAsync(business);
                await context.SaveChangesAsync();
            }
        }

        public int AddNewBusiness(Business business)
        {
            using (var context = _factory.Invoke())
            {
                context.Businesses.Add(business);
                return context.SaveChanges();
            }
        }

        public Task UpdateBusinessRankAsync(BusinessRank newBusinessPermissions)
        {
            using (var context = _factory.Invoke())
            {
                context.BusinessesRanks.Update(newBusinessPermissions);
                return context.SaveChangesAsync();
            }
        }

        public Task RemoveBusinessAsync(Business business)
        {
            using (var context = _factory.Invoke())
            {
                context.Businesses.Remove(business);
                return context.SaveChangesAsync();
            }
        }
    }
}
