using System;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Services.Businesses
{
    public class BusinessFactory
    {
        /// <summary>
        /// Creates business depeding of the business type
        /// </summary>
        /// <param name="businessType"></param>
        /// <returns></returns>
        public Business CreateBusiness(BusinessType businessType)
        {
            switch (businessType)
            {
                case BusinessType.Mechanic:
                    return new MechanicBusiness();
                case BusinessType.Restaurant:
                    return new RestaurantBusiness();
                case BusinessType.Pub:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
