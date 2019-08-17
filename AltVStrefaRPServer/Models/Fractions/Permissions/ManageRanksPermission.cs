namespace AltVStrefaRPServer.Models.Fractions.Permissions
{
    public class ManageRanksPermission : FractionPermission
    {
        public override string Name { get; set; } = "Zarządanie rangami";
        public override string Description { get; set; } = "Możliwość zarządzania rangami(usuwanie, dodawanie, zmiana uprawnień)";
        public override bool HasPermission { get; set; } = true;

        public ManageRanksPermission(bool hasPermission) : base(hasPermission) { }
    }
}
