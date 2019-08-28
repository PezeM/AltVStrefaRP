using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models.Houses;

namespace AltVStrefaRPServer.Services.Housing
{
    public class InteriorDatabaseService : IInteriorDatabaseService
    {
        private readonly Func<ServerContext> _factory;

        public InteriorDatabaseService(Func<ServerContext> factory)
        {
            _factory = factory;
        }
        
        public IEnumerable<Interior> GetAllInteriors()
        {
            using (var context = _factory.Invoke())
            {
                return context
                    .Interiors
                    .ToList();
            }
        }

        public async Task AddNewInteriorAsync(Interior interior)
        {
            using (var context = _factory.Invoke())
            {
                await context.Interiors.AddAsync(interior);
                await context.SaveChangesAsync();
            }
        }

        public void AddNewInterior(Interior interior)
        {
            using (var context = _factory.Invoke())
            {
                context.Interiors.Add(interior);
                context.SaveChanges();
            }
        }

        public void AddNewInteriors(IEnumerable<Interior> newInteriors)
        {
            using (var context = _factory.Invoke())
            {
                context.Interiors.AddRange(newInteriors);
                context.SaveChanges();
            }
        }

        public async Task SaveInteriorAsync(Interior interior)
        {
            using (var context = _factory.Invoke())
            {
                context.Interiors.Update(interior);
                await context.SaveChangesAsync();
            }
        }

        public void SaveInterior(Interior interior)
        {
            using (var context = _factory.Invoke())
            {
                context.Interiors.Update(interior);
                context.SaveChanges();
            }
        }

        public async Task RemoveInteriorAsync(Interior interior)
        {
            using (var context = _factory.Invoke())
            {
                context.Interiors.Update(interior);
                await context.SaveChangesAsync();
            }
        }

        public void RemoveInterior(Interior interior)
        {
            using (var context = _factory.Invoke())
            {
                context.Interiors.Remove(interior);
                context.SaveChanges();
            }
        }
    }
}