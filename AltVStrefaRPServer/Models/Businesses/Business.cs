using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Enums;

namespace AltVStrefaRPServer.Models.Businesses
{
    public class Business : IMoney
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string BusinessName { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Money { get; set; }
        public virtual int MaxMembersCount { get; set; } = 20;
        public virtual int MaxRanksCount { get; set; } = 5;
        public DateTime CreatedAt { get; set; }
        public BusinessType Type { get; set; }
        public int EmployeesCount => Employees.Count;
        public ICollection<Character> Employees { get; set; }
        public ICollection<BusinessRank> BusinessRanks { get; set; }

        public virtual byte BlipModel { get; protected set; }
        public virtual string BlipName { get; protected set; }
        public virtual byte BlipColor { get; protected set; }

        public virtual IBlip Blip { get; set; }

        public Position GetPosition()
        {
            return new Position(X, Y, Z);
        }

        public void SetPosition(Position position)
        {
            X = position.X;
            Y = position.Y;
            Z = position.Z;
        }

        public bool CanAddNewMember(Character newEmployee)
        {
            if (EmployeesCount >= MaxMembersCount) return false;
            if (newEmployee.Business != null) return false;
            if (newEmployee.Business == this) return false;
            return true;
        }

        public void AddNewMember(Character newEmployee)
        {
            Employees.Add(newEmployee);
        }

        public bool SetDefaultRank(Character employee)
        {
            var defaultRank = BusinessRanks.FirstOrDefault(q => q.IsDefaultRank);
            if (defaultRank == null) return false;

            employee.BusinessRank = defaultRank.Id;
            return true;
        }

        /// <summary>
        /// Check if character works at this business
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsWorkingHere(Character character) => character.Business.Id == Id;
    }
}
