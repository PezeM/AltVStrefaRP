namespace AltVStrefaRPServer.Models.Dto
{
    public class BusinessNewRankDto
    {
        public string RankName { get; set; }
        public BusinessNewRankPermissionsDto Permissions { get; set; } 
    }

    public class BusinessNewRankPermissionsDto
    {
        public bool HaveVehicleKeys { get; set; }
        public bool HaveBusinessKeys { get; set; } 
        public bool CanOpenBusinessMenu { get; set; }
        public bool CanOpenBusinessInventory { get; set; }
        public bool CanInviteNewMembers { get; set; }
        public bool CanManageRanks { get; set; }
        public bool CanManageEmployess { get; set; }
    }
}
