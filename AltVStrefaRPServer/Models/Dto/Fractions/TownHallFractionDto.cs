namespace AltVStrefaRPServer.Models.Dto.Fractions
{
    public class TownHallFractionDto
    {
        public int id {get;set;}
        public float money {get;set;}
        public int employeesCount {get; set;}
        public int rolesCount {get;set;}
        public string creationDate {get;set;}
        public float vehicleTax {get;set;}
        public float propertyTax {get;set;}
        public float gunTax {get;set;}
        public float globalTax {get;set;}
    }
}
