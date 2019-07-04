namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public abstract class BaseItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StackSize { get; set; }

        protected BaseItem(){}

        public BaseItem(string name, int stackSize)
        {
            Name = name;
            StackSize = stackSize;
        }

        public abstract bool UseItem(Character character);
    }
}
