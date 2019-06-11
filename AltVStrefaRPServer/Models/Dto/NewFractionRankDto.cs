namespace AltVStrefaRPServer.Models.Dto
{
    public class NewFractionRankDto
    {
        public string RankName { get; set; }
        public FractionNewRankPermissionsDto Permissions { get; set; }
    }

    public class FractionNewRankPermissionsDto
    {
        public bool CanOpenFractionMenu { get; set; }
        public bool HaveVehicleKeys { get; set; }
        public bool HaveFractionKeys { get; set; } 
        public bool CanManageRanks { get; set; }
        public bool CanManageEmployees { get; set; }
    }
}
