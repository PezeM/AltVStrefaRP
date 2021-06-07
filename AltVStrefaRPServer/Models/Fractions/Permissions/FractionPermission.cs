namespace AltVStrefaRPServer.Models.Fractions.Permissions
{
    public class FractionPermission
    {
        public int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual bool HasPermission { get; set; }

        private FractionPermission() { }

        protected FractionPermission(bool hasPermission)
        {
            HasPermission = hasPermission;
        }

        public void SetPermission(bool hasPermission)
        {
            HasPermission = hasPermission;
        }
    }
}
