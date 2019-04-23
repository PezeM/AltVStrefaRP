namespace AltVStrefaRPServer.Models.Businesses
{
    public class BusinessPermissions
    {
        public int Id { get; set; }
        public bool HaveVehicleKeys { get; set; }
        public bool HaveBusinessKeys { get; set; } 
        public bool CanOpenBusinessInventory { get; set; }
        public bool CanInviteNewMembers { get; set; }
        public bool CanKickOldMembers { get; set; }

        public int BusinessRankForeignKey { get; set; }
        public BusinessRank BusinessRank { get; set; }
    }
}
