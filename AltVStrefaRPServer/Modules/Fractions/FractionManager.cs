using System;
using System.Collections.Generic;
using AltV.Net;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Services.Fractions;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class FractionManager
    {
        private Dictionary<int, Fraction> _fractions;
        private IFractionDatabaseService _fractionDatabaseService;

        public FractionManager(IFractionDatabaseService fractionDatabaseService)
        {
            // Load all fractions from database
            _fractionDatabaseService = fractionDatabaseService;

            _fractions = new Dictionary<int, Fraction>();

            //CreateTestFraction();
            Initialize();
        }

        public void Initialize()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var fraction in _fractionDatabaseService.GetAllFractionsList())
            {
                _fractions.Add(fraction.Id, fraction);
                Alt.Log($"Added fraction {fraction.Name} of type {fraction.GetType()}");
            }
            Alt.Log($"Added {_fractions.Count} fractions in {Time.GetTimestampMs() - startTime}ms.");
        }

        //public void CreateTestFraction()
        //{
        //    var fraction = new TownHallFraction()
        //    {
        //        CreationDate = DateTime.Now,
        //        Description = "Urząd miasta los santos",
        //        Name = "Urząd miasta",
        //        Money = 10000f,
        //        X = 250.14f,
        //        Y = 380.14f,
        //        Z = 250.177f
        //    };

        //    fraction._fractionRanks = new List<FractionRank>
        //    {
        //        new FractionRank
        //        {
        //            IsDefaultRank = false,
        //            IsHighestRank = true,
        //            RankName = "Burmistrz",
        //            Permissions = new FractionRankPermissions
        //            {
        //                CanManageEmployees = true,
        //                CanManageRanks = true,
        //                HaveFractionKeys = true,
        //                HaveVehicleKeys = true,
        //                CanOpenFractionMenu = true
        //            }
        //        },
        //        new FractionRank
        //        {
        //            IsDefaultRank = true,
        //            IsHighestRank = false,
        //            RankName = "Pracownik",
        //            Permissions = new FractionRankPermissions
        //            {
        //                CanOpenFractionMenu = true,
        //                CanManageEmployees = false,
        //                CanManageRanks = false,
        //                HaveVehicleKeys = true,
        //                HaveFractionKeys =  true
        //            }
        //        }
        //    };


        //    fraction.SetVehicleTax(0.30f);
        //    fraction.SetPropertyTax(0.25f);
        //    fraction.SetGunTaxk(0.25f);
        //    fraction.SetGlobalTax(0.10f);
        //    _fractionDatabaseService.AddNewFraction(fraction);
        //    Alt.Log($"Added new fraction Name: {fraction.Name} with ID: {fraction.Id}");
        //}

        public bool TryToGetFraction(int fractionId, out Fraction fraction)
            => _fractions.TryGetValue(fractionId, out fraction);

        public bool TryToGetFraction(Character character, out Fraction fraction) 
            => _fractions.TryGetValue(character.CurrentFractionId.GetValueOrDefault(), out fraction);
    }
}
