using System.Collections.Generic;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions.Permissions;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class SamsFraction : Fraction
    {
        protected SamsFraction() : base() { }
        public SamsFraction(string name, string description, float money, Position position) : base(name, description, money, position) { }

        protected override void GenerateDefaultRanks()
        {
            var highestRank = new FractionRank
            {
                RankType = RankType.Highest,
                RankName = "SMAS Chief",
                Permissions = new List<FractionPermission>
                {
                    new InventoryPermission(true),
                    new ManageEmployeesPermission(true),
                    new ManageRanksPermission(true),
                    new OpenMenuPermission(true),
                    new VehiclePermission(true)
                }
            };
            _fractionRanks.Add(highestRank);

            var defaultRank = new FractionRank
            {
                RankType = RankType.Default,
                RankName = "Paramedic",
                Permissions = GenerateNewPermissions()
            };
            _fractionRanks.Add(defaultRank);
        }
    }
}
