namespace AltVStrefaRPServer.Models.Inventory.Items
{
    public abstract class BaseItem
    {
        public int Id { get; set; }
        public string Name { get; protected set; }
        public int StackSize { get; protected set; }
        public virtual string Description { get; protected set; } = "Brak opisu";

        protected BaseItem() { }

        protected BaseItem(string name, int stackSize)
        {
            Name = name;
            StackSize = stackSize;
        }
        
        protected BaseItem(BaseItem baseItem)
        {
            Id = baseItem.Id;
            Name = baseItem.Name;
            StackSize = baseItem.StackSize;
            ChangeItemDescription(baseItem.Description);
        }

        public void ChangeItemDescription(string description)
        {
            Description = description;
        }
        
        public abstract bool UseItem(Character character);

        public BaseItem ShallowClone()
        {
            return (BaseItem)MemberwiseClone();
        }

        public static T ShallowClone<T>(T original) where T : BaseItem
        {
            var newBaseItem = (T)original.ShallowClone();
            newBaseItem.Id = 0;
            return newBaseItem;
        }

        public abstract BaseItem Copy();
    }
}
