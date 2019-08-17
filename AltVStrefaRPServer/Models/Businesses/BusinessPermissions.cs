namespace AltVStrefaRPServer.Models.Businesses
{
    public class BusinessPermissions
    {
        public int Id { get; private set; }
        public bool HaveVehicleKeys { get; set; }
        public bool HaveBusinessKeys { get; set; }
        public bool CanOpenBusinessMenu { get; set; }
        public bool CanOpenBusinessInventory { get; set; }
        public bool CanInviteNewMembers { get; set; }
        public bool CanManageRanks { get; set; }
        public bool CanManageEmployess { get; set; }

        public int BusinessRankForeignKey { get; private set; }
        public BusinessRank BusinessRank { get; private set; }
    }
}
