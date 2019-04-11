using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Services.Businesses;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Modules.Businesses
{
    public class BusinessManager
    {
        private IBusinessService _businessService;
        private Dictionary<int, Business> Businesses = new Dictionary<int, Business>();
        private ServerContext _serverContext;
        private BusinessFactory _businessFactory;

        public BusinessManager(IBusinessService businessService, ServerContext serverContext)
        {
            _businessService = businessService;
            _serverContext = serverContext;
            _businessFactory = new BusinessFactory();

            LoadBusinesses();
        }

        private void LoadBusinesses()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var business in _serverContext.Businesses.AsNoTracking().ToList())
            {
                //Businesses.TryAdd(business.Id, _businessFactory.CreateBusiness(business));
                Businesses.TryAdd(business.Id, business);
                //_businessFactory.CreateBusiness(business);
            }
            Alt.Log($"Loaded {Businesses.Count} businesses from database in {Time.GetTimestampMs() - startTime}ms.");
        }

        public Business GetBusiness(int businessId) => Businesses.ContainsKey(businessId) ? Businesses[businessId] : null;

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
        /// Create new business and save it to database
        /// </summary>
        /// <param name="businessType">Type of the business <see cref="BusinessType"/></param>
        /// <param name="position">Position where the business will be located</param>
        /// <param name="name">Name of the business</param>
        /// <returns></returns>
        public bool CreateNewBusiness(BusinessType businessType, Position position, string name)
        {
            if(businessType == BusinessType.None || string.IsNullOrEmpty(name)) return false;
            var business = _businessFactory.CreateBusiness(businessType, position, name);
            _businessService.AddNewBusiness(business);
            return true;
        }
    }
}
