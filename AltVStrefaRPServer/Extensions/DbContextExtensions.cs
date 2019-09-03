using System.Linq;
using AltVStrefaRPServer.Database;
using Microsoft.EntityFrameworkCore;

namespace AltVStrefaRPServer.Extensions
{
    public static class DbContextExtensions
    {
        public static void DetachLocal<TEntity>(this ServerContext context, TEntity entity, int entityId) where TEntity : class
        {
            var local = context.Set<TEntity>()
                .Local
                .FirstOrDefault(entry => Equals(entry, entity));

            if (local != null)
            {
                context.Entry(local).State = EntityState.Detached;
            }

            context.Entry(entity).State = EntityState.Modified;
        }
    }
}
