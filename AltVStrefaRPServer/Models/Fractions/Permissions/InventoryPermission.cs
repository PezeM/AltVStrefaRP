namespace AltVStrefaRPServer.Models.Fractions.Permissions
{
    public class InventoryPermission : FractionPermission
    {
        public override string Name { get; set; } = "Otwieranie inwentarza";
        public override string Description { get; set; } = "Możliwość otwierania wspólnego inwentarza.";
        public override bool HasPermission { get; protected set; } = true;

        public InventoryPermission(bool hasPermission) : base(hasPermission){}
    }
}
