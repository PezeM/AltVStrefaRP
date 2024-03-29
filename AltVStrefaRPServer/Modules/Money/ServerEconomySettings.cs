﻿using AltVStrefaRPServer.Models.Server;

namespace AltVStrefaRPServer.Modules.Money
{
    public static class ServerEconomySettings
    {
        public static float VehicleTax { get; private set; } = 0.15f;
        public static float PropertyTax { get; private set; } = 0.22f;
        public static float GunTax { get; private set; } = 0.18f;
        public static float GlobalTax { get; private set; } = 0.09f;

        public static void UpdateTaxes(float vehicleTax, float propertyTax, float gunTax, float globalTax)
        {
            VehicleTax = vehicleTax;
            PropertyTax = propertyTax;
            GunTax = gunTax;
            GlobalTax = globalTax;
        }

        public static bool SetVehicleTax(float newTax)
        {
            if (newTax <= AppSettings.Current.ServerConfig.EconomySettings.VehicleTax.Max &&
                newTax >= AppSettings.Current.ServerConfig.EconomySettings.VehicleTax.Min)
            {
                VehicleTax = newTax;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SetPropertyTax(float newTax)
        {
            if (newTax <= AppSettings.Current.ServerConfig.EconomySettings.PropertyTax.Max &&
                newTax >= AppSettings.Current.ServerConfig.EconomySettings.PropertyTax.Min)
            {
                PropertyTax = newTax;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SetGunTax(float newTax)
        {
            if (newTax <= AppSettings.Current.ServerConfig.EconomySettings.GunTax.Max &&
                newTax >= AppSettings.Current.ServerConfig.EconomySettings.GunTax.Min)
            {
                GunTax = newTax;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool SetGlobalTax(float newTax)
        {
            if (newTax <= AppSettings.Current.ServerConfig.EconomySettings.GlobalTax.Max &&
                newTax >= AppSettings.Current.ServerConfig.EconomySettings.GlobalTax.Min)
            {
                GlobalTax = newTax;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
