using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models.Fractions;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Services.Fractions
{
    public class FractionDatabaseService : IFractionDatabaseService
    {
        private readonly ServerContext _serverContext;

        public FractionDatabaseService(ServerContext serverContext)
        {
            _serverContext = serverContext;
        }

        public List<Fraction> GetAllFractionsList()
        {
            return _serverContext.Fractions
                .Include(f => f.FractionRanks)
                .ToList();
        }

        public Task<List<Fraction>> GetAllFractionsListAsync()
        {
            return _serverContext.Fractions
                .Include(f => f.FractionRanks)
                .ToListAsync();
        }

        public Fraction GetFractionById(int fractionId)
        {
            return _serverContext.Fractions.Find(fractionId);
        }

        public Task<Fraction> GetFractionByIdAsync(int fractionId)
        {
            return _serverContext.Fractions.FindAsync(fractionId);
        }

        public void UpdateFraction(Fraction fraction)
        {
            _serverContext.Fractions.Update(fraction);
            _serverContext.SaveChanges();
        }

        public Task UpdateFractionAsync(Fraction fraction)
        {
            _serverContext.Fractions.Update(fraction);
            return _serverContext.SaveChangesAsync();
        }
    }
}
