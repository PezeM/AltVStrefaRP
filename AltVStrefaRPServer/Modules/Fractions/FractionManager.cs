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

        public bool TryToGetFraction(int fractionId, out Fraction fraction)
            => _fractions.TryGetValue(fractionId, out fraction);

        public bool TryToGetFraction(Character character, out Fraction fraction) 
            => _fractions.TryGetValue(character.CurrentFractionId.GetValueOrDefault(), out fraction);
    }
}
