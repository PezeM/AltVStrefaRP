using AltVStrefaRPServer.Models.Houses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Housing
{
    public class InteriorMap : IEntityTypeConfiguration<Interior>
    {
        public void Configure(EntityTypeBuilder<Interior> builder)
        {
            builder.Ignore(i => i.Colshape);
            builder.Ignore(i => i.Marker);

            builder.HasMany(i => i.Houses)
                .WithOne(h => h.Interior)
                .HasForeignKey(f => f.InteriorId);

            builder.HasMany(i => i.HotelRooms)
                .WithOne(hR => hR.Interior)
                .HasForeignKey(hR => hR.InteriorId);
        }
    }
}