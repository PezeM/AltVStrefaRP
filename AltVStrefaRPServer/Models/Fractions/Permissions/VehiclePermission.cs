namespace AltVStrefaRPServer.Models.Fractions.Permissions
{
    public class VehiclePermission : FractionPermission
    {
        public override string Name { get; set; } = "Korzystanie z pojazdów";
        public override string Description { get; set; } = "Czy posiada możliwość korzystania z pojazdów";
        public override bool HasPermission { get; set; } = true;

        public VehiclePermission(bool hasPermission) : base(hasPermission) { }
    }
}
