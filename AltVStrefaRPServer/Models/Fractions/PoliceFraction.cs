using AltV.Net.Data;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class PoliceFraction : Fraction
    {
        public override int BlipColor { get; protected set; } = 1;
        public override int BlipSprite { get; protected set; } = 60;
        public override string BlipName { get; protected set; } = "Policja";

        protected PoliceFraction() : base() { }
        public PoliceFraction(string name, string description, float money, Position position) : base(name, description, money, position) { }

        protected override void GenerateDefaultRanks()
        {
            var highestRank = new FractionRank("Chief of Police", RankType.Highest, 100, new List<FractionPermission>
            {
                new InventoryPermission(true),
                new ManageEmployeesPermission(true),
                new ManageRanksPermission(true),
                new OpenMenuPermission(true),
                new VehiclePermission(true)
            });
            _fractionRanks.Add(highestRank);

            var defaultRank = new FractionRank("Officer I", RankType.Default, 0, GenerateNewPermissions());
            _fractionRanks.Add(defaultRank);
        }
    }
}
