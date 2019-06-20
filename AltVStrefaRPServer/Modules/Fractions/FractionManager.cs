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
            _fractionDatabaseService = fractionDatabaseService;
            _fractions = new Dictionary<int, Fraction>();

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
