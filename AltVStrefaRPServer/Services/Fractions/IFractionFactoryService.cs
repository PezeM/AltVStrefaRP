using AltVStrefaRPServer.Models.Fractions;

namespace AltVStrefaRPServer.Services.Fractions
{
    public interface IFractionFactoryService
    {
        TownHallFraction CreateDefaultTownHallFraction(IFractionDatabaseService fractionDatabaseService);
        PoliceFraction CreateDefaultPoliceFraction(IFractionDatabaseService fractionDatabaseService);
        SamsFraction CreateDefaultSamsFraction(IFractionDatabaseService fractionDatabaseService);
    }
}
