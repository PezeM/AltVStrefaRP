using System.Collections.Generic;
using System.Linq;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions.Permissions;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class FractionRank
    {
        public int Id { get; set; }
        public string RankName { get; set; }
        public int Priority { get; private set; } = 0;
        public RankType RankType { get; set; }
        public ICollection<FractionPermission> Permissions { get; set; } = new List<FractionPermission>();
        public Fraction Fraction { get; set; }

        public bool AddNewPermission(FractionPermission permission)
        {
            if (!Permissions.Contains(permission))
            {
                Permissions.Add(permission);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemovePermission(FractionPermission permission)
        {
            return Permissions.Remove(permission);
        }

        public FractionPermission GetPermission<TPermission>() where TPermission : FractionPermission
        {
            return Permissions.FirstOrDefault(p => p is TPermission);
        }

        public bool TryToGetPermission<TPermission>(out TPermission permission) where TPermission : FractionPermission
        {
            permission = (TPermission)Permissions.FirstOrDefault(p => p is TPermission);
            return permission == null ? false : true;
        }

        public bool HasPermission<TPermission>() where TPermission : FractionPermission
        {
            return (Permissions.FirstOrDefault(q => q is TPermission)?.HasPermission) ?? false;
        }

        public bool SetPriority(int priority)
        {
            if (priority > 0 && priority <= 100)
            {
                Priority = priority;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
