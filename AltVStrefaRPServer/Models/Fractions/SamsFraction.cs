using AltV.Net.Data;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class SamsFraction : Fraction
    {
        protected SamsFraction() : base() { }
        public SamsFraction(string name, string description, float money, Position position) : base(name, description, money, position) { }

        protected override void GenerateDefaultRanks()
        {
            var highestRank = new FractionRank("SAMS Chief", RankType.Highest, 100, new List<FractionPermission>
            {
                new InventoryPermission(true),
                new ManageEmployeesPermission(true),
                new ManageRanksPermission(true),
                new OpenMenuPermission(true),
                new VehiclePermission(true)
            });
            _fractionRanks.Add(highestRank);

            var defaultRank = new FractionRank("Paramedic", RankType.Default, 0, GenerateNewPermissions());
            _fractionRanks.Add(defaultRank);
        }
    }
}
