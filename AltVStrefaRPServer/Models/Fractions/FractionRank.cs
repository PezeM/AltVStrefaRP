namespace AltVStrefaRPServer.Models.Fractions
{
    public class FractionRank
    {
        public int Id { get; set; }
        public string RankName { get; set; }
        public bool IsDefaultRank { get; set; }
        public bool IsHighestRank { get; set; }
        public Fraction Fraction { get; set; }
        public FractionRankPermissions Permissions { get; set; }
    }
}
