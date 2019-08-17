namespace AltVStrefaRPServer.Models.Fractions.Permissions
{
    public class TownHallActionsPermission : FractionPermission
    {
        public override string Name { get; set; } = "Wykonywanie pracy urzędnika";
        public override string Description { get; set; } = "Możliwość wykonywania pracy urzędnika(rejestrowanie pojazdów, wyrabianie dowodów)";
        public override bool HasPermission { get; set; } = true;

        public TownHallActionsPermission(bool hasPermission) : base(hasPermission) { }
    }
}
