using AltV.Net.Data;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Models.Server;

namespace AltVStrefaRPServer.Services.Fractions
{
    public class FractionFactoryService : IFractionFactoryService
    {
        public PoliceFraction CreateDefaultPoliceFraction(IFractionDatabaseService fractionDatabaseService)
        {
            var policeFraction = new PoliceFraction("Departament Policji", "Departament Policji", 5000.00f, new Position(200, 300, 400));
            fractionDatabaseService.AddNewFraction(policeFraction);
            return policeFraction;
        }

        public SamsFraction CreateDefaultSamsFraction(IFractionDatabaseService fractionDatabaseService)
        {
            var samsFraction = new SamsFraction("SAMS", "San Andreas Medical Services", 5000.00f, new Position(300, 400, 500));
            fractionDatabaseService.AddNewFraction(samsFraction);
            return samsFraction;
        }

        public TownHallFraction CreateDefaultTownHallFraction(IFractionDatabaseService fractionDatabaseService)
        {
            var defaultTaxes = AppSettings.Current.ServerConfig.EconomySettings;
            var townHallFraction = new TownHallFraction("Urząd miasta", "Urząd miasta Los Santos", 10000.00f, new Position(100, 200, 300), // Some random for now
                defaultTaxes.VehicleTax.Default, defaultTaxes.PropertyTax.Default, defaultTaxes.GunTax.Default, defaultTaxes.GlobalTax.Default);
            fractionDatabaseService.AddNewFraction(townHallFraction);
            return townHallFraction;
        }
    }
}
