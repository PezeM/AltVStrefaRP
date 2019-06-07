using System.Collections.Generic;
using System.Threading.Tasks;
using AltVStrefaRPServer.Models.Fractions;

namespace AltVStrefaRPServer.Services.Fractions
{
    public interface IFractionDatabaseService
    {
        Task<List<Fraction>> GetAllFractionsListAsync();
        List<Fraction> GetAllFractionsList();
        Task<Fraction> GetFractionByIdAsync(int fractionId);
        Fraction GetFractionById(int fractionId);
        Task UpdateFractionAsync(Fraction fraction);
        void UpdateFraction(Fraction fraction);
    }
}
