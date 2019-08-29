namespace AltVStrefaRPServer.Models.Inventory.Items.Keys
{
    public class HouseKeyItem : KeyItem
    {
        public string LockPattern { get; set; }
        public override string Description { get; protected set; } = "Klucze do mieszkania";

        public HouseKeyItem(string model, string lockPatterm) : base(model)
        {
            LockPattern = lockPatterm;
        }
        
        public override bool UseItem(Character character)
        {
            return true;
        }
        
        public override BaseItem Copy() => (HouseKeyItem) MemberwiseClone();
    }
}