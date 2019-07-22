using System;
using System.Collections.Generic;
using AltV.Net;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Services.Fractions;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class FractionsManager
    {
        private Dictionary<int, Fraction> _fractions;
        private readonly IFractionFactoryService _fractionFactoryService;
        private readonly IFractionDatabaseService _fractionDatabaseService;
        private TownHallFraction _townHallFraction;
        private PoliceFraction _policeFraction;
        private SamsFraction _samsFraction;

        public FractionsManager(IFractionDatabaseService fractionDatabaseService, IFractionFactoryService fractionFactoryService)
        {
            _fractionFactoryService = fractionFactoryService;
            _fractionDatabaseService = fractionDatabaseService;
            _fractions = new Dictionary<int, Fraction>();

            Initialize();

            // Create default fractions if they are not created yet
            CreateDeafultFractions();
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

        private void CreateDeafultFractions()
        {
            if (_policeFraction == null)
            {
                Alt.Server.LogWarning("Police fraction was empty. Creating missing fractions.");
               _policeFraction = _fractionFactoryService.CreateDefaultPoliceFraction(_fractionDatabaseService);
               _fractions.Add(_policeFraction.Id, _policeFraction);
               Alt.Log($"Created police fraction with id {_policeFraction.Id} and position {_policeFraction.GetPosition()}");
            } 
            if (_samsFraction == null)
            {
                Alt.Server.LogWarning("SAMS fraction was empty. Creating missing fractions.");
                _samsFraction = _fractionFactoryService.CreateDefaultSamsFraction(_fractionDatabaseService);
                _fractions.Add(_samsFraction.Id, _samsFraction);
                Alt.Log($"Created sams fraction with id {_samsFraction.Id} and position {_samsFraction.GetPosition()}");
            }
            if (_townHallFraction == null)
            {
                Alt.Server.LogWarning("Townhall fraction was empty. Creating missing fractions.");
                _townHallFraction = _fractionFactoryService.CreateDefaultTownHallFraction(_fractionDatabaseService);
                _fractions.Add(_townHallFraction.Id, _townHallFraction);
                Alt.Log($"Created townhall fraction with id {_townHallFraction.Id} and position {_townHallFraction.GetPosition()}");
            }
        }
    }
}
