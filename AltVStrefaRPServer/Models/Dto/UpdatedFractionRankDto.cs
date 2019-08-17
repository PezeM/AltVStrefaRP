using AltVStrefaRPServer.Models.Fractions.Permissions;
using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Dto
{
    public class UpdatedFractionRankDto
    {
        public int Id { get; set; }
        public string RankName { get; set; }
        public int Priority { get; set; }
        public int RankType { get; set; }
        public List<FractionPermission> Permissions { get; set; }
    }
}
