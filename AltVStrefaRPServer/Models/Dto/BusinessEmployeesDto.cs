using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Dto
{
    public class BusinessEmployeesDto
    {
        public List<BusinessRanksDto> BusinessRanks { get; set; }
    }

    public class BusinessRanksDto
    {
        public int Id { get; set; }
        public bool IsDefaultRank { get; set; }
        public bool IsOwnerRank { get; set; }
        public string RankName { get; set; }
    }
}
