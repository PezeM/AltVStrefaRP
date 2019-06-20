namespace AltVStrefaRPServer.Models.Fractions
{
    public class FractionRankPermissions
    {
        public int Id { get; set; }
        public bool CanOpenFractionMenu { get; set; }
        public bool HaveVehicleKeys { get; set; }
        public bool HaveFractionKeys { get; set; } 
        public bool CanManageRanks { get; set; }
        public bool CanManageEmployees { get; set; }
        public bool CanMakeAdvancedActions { get; set; }

        public int FractionRankFK { get; set; }
        public FractionRank FractionRank { get; set; }
    }
}
