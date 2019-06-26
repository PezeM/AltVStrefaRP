namespace AltVStrefaRPServer.Models.Server
{
    public class EconomySettings
    {
        public TaxSettings VehicleTaxSettings { get; set; }
        public TaxSettings PropertyTax { get; set; }
        public TaxSettings GunTax { get; set; }
        public TaxSettings GlobalTax { get; set; }
    }
}