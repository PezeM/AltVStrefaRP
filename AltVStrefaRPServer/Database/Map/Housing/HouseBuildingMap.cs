using AltVStrefaRPServer.Models.Houses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map.Housing
{
    public class HouseBuildingMap : IEntityTypeConfiguration<HouseBuilding>
    {
        public void Configure(EntityTypeBuilder<HouseBuilding> builder)
        {
            builder.Ignore(h => h.Colshape);
        }
    }
}