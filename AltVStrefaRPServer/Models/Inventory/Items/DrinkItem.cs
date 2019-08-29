namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class DrinkItem : Consumable
    {
        public DrinkItem(string name, int stackSize, string model, ushort value) : base(name, stackSize, model, value) { }

        public override bool UseItem(Character character)
        {
            // Decrease thirst or someshit
            return true;
        }

        public override BaseItem Copy() => (DrinkItem)MemberwiseClone();
    }
}
