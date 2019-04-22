using System;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Services.Businesses
{
    public class BusinessFactory
    {
        public Business CreateBusiness(BusinessType businessType, Position position, string name)
        {
            switch (businessType)
            {
                case BusinessType.Mechanic:
                    return CreateDefaultMechanicBusiness(position, name);
                case BusinessType.Restaurant:
                    return CreateDefaultRestaurantBussiness(position, name);
                case BusinessType.Pub:
                    return CreateDefaultPubBusiness(position, name);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Creates business depeding of the business type
        /// </summary>
        /// <param name="businessData"></param>
        /// <returns></returns>
        public Business CreateBusiness(Business businessData)
        {
            switch (businessData.Type)
            {
                case BusinessType.Mechanic:
                    return CreateMechanicBusiness(businessData);
                case BusinessType.Restaurant:
                    return CreateRestaurantBusiness(businessData);
                case BusinessType.Pub:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        private MechanicBusiness CreateDefaultMechanicBusiness(Position position, string name)
        {
            var business = new MechanicBusiness
            {
                BusinessName = name,
                Money = 250,
                X = position.X,
                Y = position.X,
                Z = position.Z,
                CreatedAt = DateTime.Now,
                Type = BusinessType.Mechanic,
            };
            //business.Blip = CreateBlip(business);
            return business;
        }

        private RestaurantBusiness CreateDefaultRestaurantBussiness(Position position, string name)
        {
            var business = new RestaurantBusiness()
            {
                BusinessName = name,
                Money = 250,
                X = position.X,
                Y = position.X,
                Z = position.Z,
                CreatedAt = DateTime.Now,
                Type = BusinessType.Mechanic,
            };
            //business.Blip = CreateBlip(business);
            return business;
        }

        private Business CreateDefaultPubBusiness(Position position, string name)
        {
            var business = new PubBusiness()
            {
                BusinessName = name,
                Money = 250,
                X = position.X,
                Y = position.X,
                Z = position.Z,
                CreatedAt = DateTime.Now,
                Type = BusinessType.Mechanic,
            };
            //business.Blip = CreateBlip(business);
            return business;
        }

        private Business CreateRestaurantBusiness(Business business)
        {
            return new MechanicBusiness
            {
                Id = business.Id,
                OwnerId = business.OwnerId,
                BusinessName = business.BusinessName,
                Money = business.Money,
                X = business.X,
                Y = business.Y,
                Z = business.Z,
                CreatedAt = business.CreatedAt,
                Type = business.Type,
                //Blip = CreateBlip(business)
            };
        }

        private MechanicBusiness CreateMechanicBusiness(Business business)
        {
            return new MechanicBusiness
            {
                Id = business.Id,
                OwnerId = business.OwnerId,
                BusinessName = business.BusinessName,
                Money = business.Money,
                X = business.X,
                Y = business.Y,
                Z = business.Z,
                CreatedAt = business.CreatedAt,
                Type = business.Type,
                //Blip = CreateBlip(business)
            };
        }

        private IBlip CreateBlip(Business business)
        {
            var blip = Alt.CreateBlip(BlipType.Object, business.GetPosition());
            blip.Color = business.BlipColor;
            blip.Sprite = business.BlipModel;
            return blip;
        }
    }
}
