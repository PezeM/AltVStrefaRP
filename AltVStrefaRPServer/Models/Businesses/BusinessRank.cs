namespace AltVStrefaRPServer.Models.Businesses
{
    public class BusinessRank
    {
        public int Id { get; set; }
        public bool IsDefaultRank { get; set; }
        public string RankName { get; set; }
        public Business Business { get; set; }
        public BusinessPermissions Permissions { get; set; }
    }
}
