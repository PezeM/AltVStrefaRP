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
        private TownHallFraction _townHallFraction;
        private PoliceFraction _policeFraction;
        private SamsFraction _samsFraction;

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
                if (fraction.GetType() == typeof(TownHallFraction))
                {
                    _townHallFraction = (TownHallFraction)fraction;
                } 
                else if (fraction.GetType() == typeof(PoliceFraction))
                {
                    _policeFraction = (PoliceFraction)fraction;
                }
                else if (fraction.GetType() == typeof(SamsFraction))
                {
                    _samsFraction = (SamsFraction)fraction;
                }
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

        public Fraction GetFraction<T>() where T : Fraction
        {
            if (_townHallFraction.GetType() == typeof(T)) 
                return _townHallFraction;
            else if (_policeFraction.GetType() == typeof(T)) 
                return _policeFraction;
            else if (_samsFraction.GetType() == typeof(T)) 
                return _samsFraction;
            else 
                return null;
        }

        public bool TryToGetTownHallFraction(out TownHallFraction townHallFraction)
        {
            townHallFraction = _townHallFraction;
            return townHallFraction != null;
        }

        public bool TryToGetPoliceFraction(out PoliceFraction policeFraction)
        {
            policeFraction = _policeFraction;
            return policeFraction != null;
        }

        public bool TryToGetSamsFraction(out SamsFraction samsFraction)
        {
            samsFraction = _samsFraction;
            return samsFraction != null;
        }
    }
}
