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

        public IEnumerable<Fraction> GetAllFractionsList()
        {
            return _serverContext.Fractions
                .Include(f => f.Employees)
                .Include(f => f.FractionRanks)
                .ThenInclude(f => f.Permissions);
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

        public async Task<int> AddNewFractionAsync(Fraction fraction)
        {
            await _serverContext.Fractions.AddAsync(fraction).ConfigureAwait(false);
            return await _serverContext.SaveChangesAsync();
        }

        public int AddNewFraction(Fraction fraction)
        {
            _serverContext.Fractions.Add(fraction);
            return _serverContext.SaveChanges();
        }

        public async Task RemoveFractionRankAsync(FractionRank fractionRank)
        {
            _serverContext.FractionRanks.Remove(fractionRank);
            await _serverContext.SaveChangesAsync();
        }
    }
}
