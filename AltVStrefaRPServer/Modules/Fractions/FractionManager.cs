using System.Collections.Generic;
using AltVStrefaRPServer.Models.Fractions;

namespace AltVStrefaRPServer.Modules.Fractions
{
    public class FractionManager
    {
        public Dictionary<int, Fraction> Fractions { get; private set; }

        public FractionManager()
        {
            // Load all fractions from database
        }

        public bool TryToGetFraction(int fractionId, out Fraction fraction)
            => Fractions.TryGetValue(fractionId, out fraction);
    }
}
