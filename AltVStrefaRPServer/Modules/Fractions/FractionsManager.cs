using System.Collections.Generic;
using AltVStrefaRPServer.Helpers;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Models.Interfaces.Managers;
using AltVStrefaRPServer.Services.Fractions;
using Microsoft.Extensions.Logging;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class FractionsManager : IFractionsManager
    {
        private Dictionary<int, Fraction> _fractions;
        private readonly ILogger<FractionsManager> _logger;
        private readonly IFractionFactoryService _fractionFactoryService;
        private readonly IFractionDatabaseService _fractionDatabaseService;
        private TownHallFraction _townHallFraction;
        private PoliceFraction _policeFraction;
        private SamsFraction _samsFraction;

        public FractionsManager(IFractionDatabaseService fractionDatabaseService, IFractionFactoryService fractionFactoryService, ILogger<FractionsManager> logger)
        {
            _fractionFactoryService = fractionFactoryService;
            _fractionDatabaseService = fractionDatabaseService;
            _fractions = new Dictionary<int, Fraction>();
            _logger = logger;

            Initialize();

            // Create default fractions if they are not created yet
            CreateDeafultFractions();
        }

        public bool TryToGetFraction<T>(int fractionId, out Fraction fraction) where T : Fraction
        {
            _fractions.TryGetValue(fractionId, out fraction);
            return fraction is T;
        }

        public bool TryToGetFraction(int fractionId, out Fraction fraction)
            => _fractions.TryGetValue(fractionId, out fraction);

        public bool TryToGetFraction(Character character, out Fraction fraction) 
            => _fractions.TryGetValue(character.CurrentFractionId.GetValueOrDefault(), out fraction);

        public Fraction GetFraction<T>() where T : Fraction
        {
            // TODO: Refactor this
            if (_townHallFraction is T)
                return _townHallFraction;
            else if (_policeFraction is T) 
                return _policeFraction;
            else if (_samsFraction is T) 
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

        private void Initialize()
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
            _logger.LogInformation("Loaded {fractionsCount} fractions from databse in {elapsedTime} ms", _fractions.Count, Time.GetTimestampMs() - startTime);
        }

        private void CreateDeafultFractions()
        {
            if (_policeFraction == null)
            {
               _logger.LogWarning("Police fraction was empty. Creating missing fractions.");
               _policeFraction = _fractionFactoryService.CreateDefaultPoliceFraction(_fractionDatabaseService);
               _fractions.Add(_policeFraction.Id, _policeFraction);
               _logger.LogInformation("Created new police fraction {@fraction}", _policeFraction);
            } 
            if (_samsFraction == null)
            {
                _logger.LogWarning("SAMS fraction was empty. Creating missing fractions.");
                _samsFraction = _fractionFactoryService.CreateDefaultSamsFraction(_fractionDatabaseService);
                _fractions.Add(_samsFraction.Id, _samsFraction);
                _logger.LogInformation("Created new sams fraction {@fraction}", _samsFraction);
            }
            if (_townHallFraction == null)
            {
                _logger.LogWarning("Townhall fraction was empty. Creating missing fractions.");
                _townHallFraction = _fractionFactoryService.CreateDefaultTownHallFraction(_fractionDatabaseService);
                _fractions.Add(_townHallFraction.Id, _townHallFraction);
                _logger.LogInformation("Created new townhall fraction {@fraction}", _townHallFraction);
            }
        }
    }
}
