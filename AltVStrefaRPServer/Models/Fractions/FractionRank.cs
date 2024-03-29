﻿using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using System.Collections.Generic;
using System.Linq;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class FractionRank
    {
        public int Id { get; set; }
        public string RankName { get; set; }
        public int Priority { get; private set; } = 0;
        public RankType RankType { get; private set; }
        public ICollection<FractionPermission> Permissions { get; private set; } = new List<FractionPermission>();
        public Fraction Fraction { get; private set; }

        private FractionRank() { }

        public FractionRank(string rankName, RankType rankType, int rankPriority, List<FractionPermission> permissions)
        {
            RankName = rankName;
            RankType = rankType;
            Permissions = permissions;
            SetPriority(rankPriority);
        }

        public bool AddNewPermission(FractionPermission permission)
        {
            if (Permissions.Any(p => p.Name == permission.Name))
                return false;

            Permissions.Add(permission);
            return true;
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
            return permission != null;
        }

        public bool HasPermission<TPermission>() where TPermission : FractionPermission
        {
            return (Permissions.FirstOrDefault(q => q is TPermission)?.HasPermission) ?? false;
        }

        public bool HasHigherPriority(FractionRank secondRank)
        {
            if (RankType == RankType.Highest) return true;
            return Priority > secondRank.Priority;
        }

        public bool SetPriority(int priority)
        {
            if (priority <= 0 || priority > 100)
                return false;

            Priority = priority;
            return true;
        }
    }
}
