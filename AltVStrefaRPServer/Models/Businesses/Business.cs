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
        public virtual int Transactions { get; set; }
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

        /// <summary>
        /// Checks if business can have more members
        /// </summary>
        /// <param name="newEmployee"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if business can have more ranks
        /// </summary>
        /// <returns></returns>
        public bool CanAddNewRank() => BusinessRanks.Count < MaxRanksCount;

        /// <summary>
        /// Returns a business rank if character is employee, otherwise returns null. 
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public bool GetBusinessRankForEmployee(Character employee, out BusinessRank businessRank)
        {
            businessRank = BusinessRanks.FirstOrDefault(r => r.Id == employee.BusinessRank);
            return businessRank != null;
        }

        /// <summary>
        /// Checks whether character is an employee at this business
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public bool IsCharacterEmployee(Character employee) => Employees.Any(q => q.Id == employee.Id);

        /// <summary>
        /// Checks whether character with given ID is an employee at this business
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        public bool IsCharacterEmployee(int characterId, out Character employee)
        {
            employee = Employees.FirstOrDefault(q => q.Id == characterId);
            return employee != null;
        }

        /// <summary>
        /// Checks whether rank with given rankId exists in business
        /// </summary>
        /// <param name="rankId"></param>
        /// <returns></returns>
        public bool CheckIfRankExists(int rankId) => BusinessRanks.Any(q => q.Id == rankId);

        public bool GetBusinessRank(int rankId, out BusinessRank businessRank)
        {
            businessRank = BusinessRanks.FirstOrDefault(r => r.Id == rankId);
            return businessRank != null;
        }

        public bool RemoveEmployee(Character employee)
        {
            if (!GetBusinessRankForEmployee(employee, out BusinessRank businessRank)) return false;
            if (businessRank.IsOwnerRank) return false;

            return Employees.Remove(employee);
        }
    }
}
