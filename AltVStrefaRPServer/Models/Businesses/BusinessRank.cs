namespace AltVStrefaRPServer.Models.Businesses
{
    public class BusinessRank
    {
        public int Id { get; private set; }
        public bool IsDefaultRank { get; set; }
        public bool IsOwnerRank { get; set; }
        public string RankName { get; set; }
        public Business Business { get; private set; }
        public BusinessPermissions Permissions { get; set; }
    }
}
