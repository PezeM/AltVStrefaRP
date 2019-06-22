namespace AltVStrefaRPServer.Models.Fractions.Permissions
{
    public class OpenTaxesPagePermission : FractionPermission
    {
        public override string Name { get; set; } = "Otwieranie menu podatkowego";
        public override string Description { get; set; } = "Możliwość otworzenia menu z podatkami.";
        public override bool HasPermission { get; protected set; } = false;
    }
}
