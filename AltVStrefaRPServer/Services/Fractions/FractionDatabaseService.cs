using System;
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
        private readonly Func<ServerContext> _factory;

        public FractionDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<Fraction> GetAllFractionsList()
        {
            using (var context = _factory.Invoke())
            {
                return context.Fractions
                    .Include(f => f.Employees)
                    .Include(f => f.FractionRanks)
                    .ThenInclude(f => f.Permissions)
                    .ToList();
            }
        }

        public Fraction GetFractionById(int fractionId)
        {
            using (var context = _factory.Invoke())
            {
                return context.Fractions.Find(fractionId);
            }
        }

        public async Task<Fraction> GetFractionByIdAsync(int fractionId)
        {
            using (var context = _factory.Invoke())
            {
                return await context.Fractions.FindAsync(fractionId);
            }
        }

        public void UpdateFraction(Fraction fraction)
        {
            using (var context = _factory.Invoke())
            {
                context.Fractions.Update(fraction);
                context.SaveChanges();
            }
        }

        public async Task UpdateFractionAsync(Fraction fraction)
        {
            using (var context = _factory.Invoke())
            {
                context.Fractions.Update(fraction);
                await context.SaveChangesAsync();
            }
        }

        public async Task<int> AddNewFractionAsync(Fraction fraction)
        {
            using (var context = _factory.Invoke())
            {
                await context.Fractions.AddAsync(fraction).ConfigureAwait(false);
                return await context.SaveChangesAsync();
            }
        }

        public int AddNewFraction(Fraction fraction)
        {
            using (var context = _factory.Invoke())
            {
                context.Fractions.Add(fraction);
                return context.SaveChanges();
            }
        }

        public async Task RemoveFractionRankAsync(FractionRank fractionRank)
        {
            using (var context = _factory.Invoke())
            {
                context.FractionRanks.Remove(fractionRank);
                await context.SaveChangesAsync();
            }
        }
    }
}
