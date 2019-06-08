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
            }
            Alt.Log($"Added {_fractions.Count} fractions in {Time.GetTimestampMs() - startTime}ms.");
        }

        //public void CreateTestFraction()
        //{
        //    var fraction = new PoliceFraction
        //    {
        //        CreationDate = DateTime.Now,
        //        Description = "Jednosta policji",
        //        Name = "LSPD",
        //        Money = 10000f,
        //        X = 150.14f,
        //        Y = 180.14f,
        //        Z = 250.177f,
        //        FractionRanks = new List<FractionRank>
        //        {
        //            new FractionRank
        //            {
        //                IsDefaultRank = true,
        //                IsHighestRank = false,
        //                RankName = "Oficer I",
        //                Permissions = new FractionRankPermissions
        //                {
        //                    CanManageEmployess = false,
        //                    CanManageRanks = false,
        //                    CanOpenFractionMenu = true,
        //                    HaveVehicleKeys = true,
        //                    HaveFractionKeys = false,
        //                }
        //            },
        //            new FractionRank
        //            {
        //                IsDefaultRank = false,
        //                IsHighestRank = true,
        //                RankName = "Chief of Police",
        //                Permissions = new FractionRankPermissions
        //                {
        //                    CanManageEmployess = true,
        //                    CanManageRanks = true,
        //                    CanOpenFractionMenu = true,
        //                    HaveVehicleKeys = true,
        //                    HaveFractionKeys = true,
        //                }
        //            },
        //        }
        //    };

        //    _fractionDatabaseService.AddNewFraction(fraction);
        //    Alt.Log($"Added new fraction Name: {fraction.Name} with ID: {fraction.Id}");
        //}

        public bool TryToGetFraction(int fractionId, out Fraction fraction)
            => _fractions.TryGetValue(fractionId, out fraction);

        public bool TryToGetFraction(Character character, out Fraction fraction) 
            => _fractions.TryGetValue(character.CurrentFractionId.GetValueOrDefault(), out fraction);
    }
}
