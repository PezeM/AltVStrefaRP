using AltVStrefaRPServer.Models.Houses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class OldHouseMap : IEntityTypeConfiguration<OldHouse>
    {
        public void Configure(EntityTypeBuilder<OldHouse> builder)
        {
            builder.Ignore(h => h.Colshape);
        }
    }
}