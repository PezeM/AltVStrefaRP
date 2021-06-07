using AltVStrefaRPServer.Models.Houses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Housing
{
    public class HotelMap : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.Ignore(h => h.Colshape);
            builder.Ignore(h => h.Marker);

            builder.HasMany(h => h.HotelRooms)
                .WithOne(hR => hR.Hotel)
                .HasForeignKey(hR => hR.HotelId);
        }
    }
}
