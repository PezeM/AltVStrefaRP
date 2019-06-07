using System.Collections.Generic;
using System.Linq;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class FractionPermission
    {
        public int Id { get; set; }
        public virtual bool HasPermission { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual string Description { get; protected set; }

        public virtual FractionRankPermissions FractionRankPermissions { get; set; }
    }

    public class FractionRankPermissions
    {
        public int Id { get; set; }
        public IEnumerable<FractionPermission> Permissions { get; private set; }
        public int FractionRankId { get; set; }
        public FractionRank FractionRank { get; set; }

        public FractionRankPermissions()
        {
            Permissions = new List<FractionPermission>();    
        }

        public FractionPermission GetPermission<TPermission>() where TPermission : FractionPermission
        {
            return Permissions.First();
        }

        public bool HasPermission<TPermission>() where TPermission : FractionPermission
        {
            return true;
        }
    }

    public class OpenFractionMenuPermission : FractionPermission
    {
        public override bool HasPermission { get; protected set; } = false;

        public override string Name { get; protected set; } = "Otwieranie menu";
        public override string Description { get; protected set; } = "Możliwość otwierania menu frakcji";

        public override FractionRankPermissions FractionRankPermissions { get; set; }
    }

    public class OpenFractionInventoryPermission : FractionPermission
    {
        public override bool HasPermission { get; protected set; } = false;

        public override string Name { get; protected set; } = "Otwieranie inwentarza";
        public override string Description { get; protected set; } = "Możliwość otwierania inwentarza frakcji";

        public override FractionRankPermissions FractionRankPermissions { get; set; }
    }

}
