using System;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Services.Businesses
{
    public class BusinessFactory
    {
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

        private Business CreateRestaurantBusiness(Business business)
        {
            return new MechanicBusiness
            {
                Id = business.Id,
                OwnerId = business.OwnerId,
                Title = business.Title,
                Money = business.Money,
                X = business.X,
                Y = business.Y,
                Z = business.Z,
                CreatedAt = business.CreatedAt,
                Type = business.Type,
                Blip = CreateBlip(business)
            };
        }

        private MechanicBusiness CreateMechanicBusiness(Business business)
        {
            return new MechanicBusiness
            {
                Id = business.Id,
                OwnerId = business.OwnerId,
                Title = business.Title,
                Money = business.Money,
                X = business.X,
                Y = business.Y,
                Z = business.Z,
                CreatedAt = business.CreatedAt,
                Type = business.Type,
                Blip = CreateBlip(business)
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
