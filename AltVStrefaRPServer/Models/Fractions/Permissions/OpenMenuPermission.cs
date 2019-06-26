namespace AltVStrefaRPServer.Models.Fractions.Permissions
{
    public class OpenMenuPermission : FractionPermission
    {
        public override string Name { get; set; } = "Otwieranie menu";
        public override string Description { get; set; } = "Czy ma możliwość otworzenia menu frakcji";
        public override bool HasPermission { get; set; } = true;

        public OpenMenuPermission(bool hasPermission) : base(hasPermission){}
    }
}
