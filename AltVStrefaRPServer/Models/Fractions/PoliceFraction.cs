﻿using AltV.Net.Data;
using System.Collections.Generic;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions.Permissions;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class PoliceFraction : Fraction
    {
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
