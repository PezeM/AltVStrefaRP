using System.Collections.Generic;
using AltVStrefaRPServer.Models.Businesses;

namespace AltVStrefaRPServer.Models.Dto
{
    public class BusinessEmployessDto
    {
        public List<BusinessRanksDto> BusinessRanks { get; set; }
        public List<BusinessEmployeeExtendedDto> BusinessEmployess { get; set; }
    }

    public class BusinessRanksDto
    {
        public int Id { get; set; }
        public bool IsDefaultRank { get; set; }
        public string RankName { get; set; }
    }

    public class BusinessEmployeeExtendedDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int Gender { get; set; }
        public int RankId { get; set; }
        public string RankName { get; set; }
    }
}
