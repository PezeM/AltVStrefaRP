using AltVStrefaRPServer.Models.Houses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class InteriorMap : IEntityTypeConfiguration<Interior>
    {
        public void Configure(EntityTypeBuilder<Interior> builder)
        {
            builder.Ignore(i => i.Colshape);

            builder.HasMany(i => i.Houses)
                .WithOne(h => h.Interior);
        }
    }
}