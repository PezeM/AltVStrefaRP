using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Enums;
using System;
using System.Collections.Generic;

namespace AltVStrefaRPServer.Services.Businesses
{
    public class BusinessFactory
    {
        public Business CreateNewBusiness(int ownerId, BusinessType businessType, Position position, string name)
        {
            switch (businessType)
            {
                case BusinessType.Mechanic:
                    return CreateDefaultMechanicBusiness(ownerId, position, name);
                case BusinessType.Restaurant:
                    return CreateDefaultRestaurantBussiness(ownerId, position, name);
                case BusinessType.Pub:
                    return CreateDefaultPubBusiness(ownerId, position, name);
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

        private MechanicBusiness CreateDefaultMechanicBusiness(int ownerId, Position position, string name)
        {
            var business = new MechanicBusiness
            {
                BusinessName = name,
                OwnerId = ownerId,
                X = position.X,
                Y = position.X,
                Z = position.Z,
                CreatedAt = DateTime.Now,
                Type = BusinessType.Mechanic,
                BusinessRanks = CreateDefaultBusinessRanks(),
                Employees = new List<Character>(),
            };
            business.Blip = CreateBlip(business);
            return business;
        }

        private RestaurantBusiness CreateDefaultRestaurantBussiness(int ownerId, Position position, string name)
        {
            var business = new RestaurantBusiness()
            {
                BusinessName = name,
                OwnerId = ownerId,
                X = position.X,
                Y = position.X,
                Z = position.Z,
                CreatedAt = DateTime.Now,
                Type = BusinessType.Mechanic,
                BusinessRanks = CreateDefaultBusinessRanks(),
                Employees = new List<Character>(),
            };
            business.Blip = CreateBlip(business);
            return business;
        }

        private Business CreateDefaultPubBusiness(int ownerId, Position position, string name)
        {
            var business = new PubBusiness()
            {
                BusinessName = name,
                OwnerId = ownerId,
                X = position.X,
                Y = position.X,
                Z = position.Z,
                CreatedAt = DateTime.Now,
                Type = BusinessType.Mechanic,
                BusinessRanks = CreateDefaultBusinessRanks(),
                Employees = new List<Character>(),
            };
            business.Blip = CreateBlip(business);
            return business;
        }

        private Business CreateRestaurantBusiness(Business business)
        {
            return new MechanicBusiness
            {
                Id = business.Id,
                OwnerId = business.OwnerId,
                BusinessName = business.BusinessName,
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
                BusinessName = business.BusinessName,
                X = business.X,
                Y = business.Y,
                Z = business.Z,
                CreatedAt = business.CreatedAt,
                Type = business.Type,
                Blip = CreateBlip(business)
            };
        }

        private List<BusinessRank> CreateDefaultBusinessRanks()
        {
            return new List<BusinessRank>
            {
                new BusinessRank
                {
                    RankName = "Właściciel",
                    IsOwnerRank = true,
                    Permissions = new BusinessPermissions
                    {
                        CanInviteNewMembers = true,
                        CanOpenBusinessInventory = true,
                        HaveBusinessKeys = true,
                        HaveVehicleKeys = true,
                        CanManageRanks = true,
                        CanOpenBusinessMenu = true,
                        CanManageEmployess = true,
                    }
                },
                new BusinessRank
                {
                    RankName = "Pracownik",
                    IsDefaultRank = true,
                    Permissions = new BusinessPermissions
                    {
                        CanInviteNewMembers = false,
                        CanOpenBusinessInventory = false,
                        HaveBusinessKeys = false,
                        HaveVehicleKeys = false,
                        CanOpenBusinessMenu = false,
                        CanManageRanks = false,
                        CanManageEmployess = false
                    }
                }
            };
        }

        private IBlip CreateBlip(Business business)
        {
            var blip = Alt.CreateBlip(BlipType.Object, business.GetPosition());
            blip.Color = (byte)business.BlipColor;
            blip.Sprite = (byte)business.BlipSprite;
            return blip;
        }
    }
}
