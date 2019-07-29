using AltVStrefaRPServer.Models.Fractions;

namespace AltVStrefaRPServer.Models.Interfaces.Managers
{
    public interface IFractionsManager : IManager<Fraction>
    {
        bool TryToGetFraction<T>(int fractionId, out Fraction fraction) where T : Fraction;
        bool TryToGetFraction(int fractionId, out Fraction fraction);
        bool TryToGetFraction(Character character, out Fraction fraction);
        Fraction GetFraction<T>() where T : Fraction;
        bool TryToGetTownHallFraction(out TownHallFraction townHallFraction);
        bool TryToGetPoliceFraction(out PoliceFraction policeFraction);
        bool TryToGetSamsFraction(out SamsFraction samsFraction);
    }
}
