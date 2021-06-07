namespace AltVStrefaRPServer.Models.Fractions.Permissions
{
    public class ManageEmployeesPermission : FractionPermission
    {
        public override string Name { get; set; } = "Zarządzanie pracownikami";
        public override string Description { get; set; } = "Możliwość zarządzania pracownikami(zapraszanie, zmiana rangi)";
        public override bool HasPermission { get; set; } = true;

        public ManageEmployeesPermission(bool hasPermission) : base(hasPermission) { }
    }
}
