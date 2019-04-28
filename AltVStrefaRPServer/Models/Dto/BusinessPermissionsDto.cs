namespace AltVStrefaRPServer.Models.Dto
{
    public class BusinessPermissionsDto
    {
        public int Id { get; set; }
        public int RankId { get; set; }
        public string RankName { get; set; }
        public bool HaveVehicleKeys { get; set; }
        public bool HaveBusinessKeys { get; set; }
        public bool CanOpenBusinessMenu { get; set; }
        public bool CanOpenBusinessInventory { get; set; }
        public bool CanInviteNewMembers { get; set; }
        public bool CanManageRanks { get; set; }
        public bool CanManageEmployees { get; set; }
    }
}
