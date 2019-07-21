using System;
using System.Collections.Generic;
using AltV.Net;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using AltVStrefaRPServer.Services.Fractions;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class FractionsManager
    {
        private Dictionary<int, Fraction> _fractions;
        private IFractionDatabaseService _fractionDatabaseService;
        private TownHallFraction _townHallFraction;
        private PoliceFraction _policeFraction;
        private SamsFraction _samsFraction;

        public FractionsManager(IFractionDatabaseService fractionDatabaseService)
        {
            _fractionDatabaseService = fractionDatabaseService;
            _fractions = new Dictionary<int, Fraction>();

            Initialize();
            //AddNewPermissions(); Was already added
        }

        public void Initialize()
        {
            var startTime = Time.GetTimestampMs();
            foreach (var fraction in _fractionDatabaseService.GetAllFractionsList())
            {
                _fractions.Add(fraction.Id, fraction);
                if (fraction is TownHallFraction townHallFraction)
                {
                    _townHallFraction = townHallFraction;
                } 
                else if (fraction is PoliceFraction policeFraction)
                {
                    _policeFraction = policeFraction;
                }
                else if (fraction is SamsFraction samsFraction)
                {
                    _samsFraction = samsFraction;
                }
            }
            Alt.Log($"Loaded {_fractions.Count} fractions in {Time.GetTimestampMs() - startTime}ms.");
        }

        private void AddNewPermissions()
        {
            foreach (var fraction in _fractions.Values)
            {
                foreach (var fractionRank in fraction.FractionRanks)
                {
                    fractionRank.AddNewPermission(new ManageEmployeesPermission(true));
                    fractionRank.AddNewPermission(new OpenMenuPermission(true));
                    fractionRank.AddNewPermission(new InventoryPermission(true));
                    fractionRank.AddNewPermission(new ManageRanksPermission(true));
                    fractionRank.AddNewPermission(new VehiclePermission(true));

                    if (fraction is TownHallFraction)
                    {
                        fractionRank.AddNewPermission(new TownHallActionsPermission(true));
                        if (fractionRank.RankType == RankType.Highest)
                        {
                            fractionRank.AddNewPermission(new TownHallActionsPermission(true));
                        }
                        else
                        {
                            fractionRank.AddNewPermission(new TownHallActionsPermission(false));
                        }
                    }
                }

                _fractionDatabaseService.UpdateFraction(fraction);
            }
        }

        public bool TryToGetFraction(int fractionId, out Fraction fraction)
            => _fractions.TryGetValue(fractionId, out fraction);

        public bool TryToGetFraction(Character character, out Fraction fraction) 
            => _fractions.TryGetValue(character.CurrentFractionId.GetValueOrDefault(), out fraction);

        public Fraction GetFraction<T>() where T : Fraction
        {
            // TODO: Refactor this
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
