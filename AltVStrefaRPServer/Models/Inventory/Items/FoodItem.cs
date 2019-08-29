namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public class FoodItem : Consumable
    {
        public FoodItem(string name, int stackSize, string model, ushort value) : base(name, stackSize, model, value) {}
        
        public override bool UseItem(Character character)
        {
            character.Player.Health += Value;
            return true;
        }

        public override BaseItem Copy() => (FoodItem)MemberwiseClone();
    }
}
