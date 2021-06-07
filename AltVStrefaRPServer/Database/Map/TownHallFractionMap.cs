using AltVStrefaRPServer.Models.Fractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltVStrefaRPServer.Database.Map
{
    public class TownHallFractionMap : IEntityTypeConfiguration<TownHallFraction>
    {
        public void Configure(EntityTypeBuilder<TownHallFraction> builder)
        {
            builder.Ignore(q => q.Taxes);
        }
    }
}
