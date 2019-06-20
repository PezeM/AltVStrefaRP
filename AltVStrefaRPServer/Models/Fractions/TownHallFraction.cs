using System;
using AltVStrefaRPServer.Modules.Money;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class TownHallFraction : Fraction
    {
        public float VehicleTax { get; private set; }
        public float PropertyTax { get; private set; }
        public float GunTax { get; private set; }
        public float GlobalTax { get; private set; }

        public override string BlipName { get; protected set; } = "Urząd miasta";

        private TownHallFraction()
        {
            ServerEconomySettings.UpdateTaxes(VehicleTax, PropertyTax, GunTax, GlobalTax);
        }

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
            return amount + tax;
        }
    }
}
