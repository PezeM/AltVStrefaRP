namespace AltVStrefaRPServer.Models.Fractions.Permissions
{
    public class ManageRanksPermission : FractionPermission
    {
        public override string Name { get; set; } = "Zarządania rangami";
        public override string Description { get; set; } = "Możliwość zarządzania rangami(usuwanie, dodawanie, zmiana uprawnień)";
        public override bool HasPermission { get; protected set; } = true;

        public ManageRanksPermission(bool hasPermission) : base(hasPermission){}
    }
}
