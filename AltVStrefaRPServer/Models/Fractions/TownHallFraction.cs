using System;
using System.Collections.Generic;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using AltVStrefaRPServer.Modules.Money;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class TownHallFraction : Fraction
    {
        public float VehicleTax { get; private set; }
        public float PropertyTax { get; private set; }
        public float GunTax { get; private set; }
        public float GlobalTax { get; private set; }
        public List<float> Taxes { get; private set; } = new List<float>(); 
        public override string BlipName { get; protected set; } = "Urząd miasta";

        protected TownHallFraction() : base()
        {
            ServerEconomySettings.UpdateTaxes(VehicleTax, PropertyTax, GunTax, GlobalTax);
        }

        public TownHallFraction(string name, string description, float money, Position position) : base(name, description, money, position) { }

        public bool SetVehicleTax(float newTax)
        {
            if(!ServerEconomySettings.SetVehicleTax(newTax)) return false;

            VehicleTax = newTax;
            return true;
        }

        public bool SetPropertyTax(float newTax)
        {
            if(!ServerEconomySettings.SetPropertyTax(newTax)) return false;

            PropertyTax = newTax;
            return true;
        }

        public bool SetGunTax(float newTax)
        {
            if(!ServerEconomySettings.SetGunTax(newTax)) return false;

            GunTax = newTax;
            return true;
        }

        public bool SetGlobalTax(float newTax)
        {
            if(!ServerEconomySettings.SetGlobalTax(newTax)) return false;

            GlobalTax = newTax;
            return true;
        }

        public float PriceAfterTax(float amount, float taxPercentage)
        {
            var tax = (float)Math.Round(amount * taxPercentage);
            Money += tax;
            Taxes.Add(tax);
            return amount + tax;
        }

        protected override void GenerateDefaultRanks()
        {
            var highestRank = new FractionRank("Burmistrz", RankType.Highest, 100, new List<FractionPermission>
            {
                new InventoryPermission(true),
                new ManageEmployeesPermission(true),
                new ManageRanksPermission(true),
                new OpenMenuPermission(true),
                new OpenTaxesPagePermission(true),
                new TownHallActionsPermission(true),
                new VehiclePermission(true)
            });
            _fractionRanks.Add(highestRank);

            var defaultRank = new FractionRank("Pracownik", RankType.Default, 0, GenerateNewPermissions());
            defaultRank.AddNewPermission(new TownHallActionsPermission(false));
            defaultRank.AddNewPermission(new OpenTaxesPagePermission(false));
            _fractionRanks.Add(defaultRank);
        }
    }
}
