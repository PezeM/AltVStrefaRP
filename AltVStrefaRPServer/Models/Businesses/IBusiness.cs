using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace AltVStrefaRPServer.Models.Businesses
{
    public interface IBusiness : IMoney, IPosition, IHaveBlip
    {
        IBlip Blip { get; set; }
        string BusinessName { get; set; }
        ICollection<BusinessRank> BusinessRanks { get; set; }
        DateTime CreatedAt { get; set; }
        ICollection<Character> Employees { get; set; }
        int EmployeesCount { get; }
        int Id { get; set; }
        int MaxMembersCount { get; set; }
        int MaxRanksCount { get; set; }
        int OwnerId { get; set; }
        int Transactions { get; set; }
        BusinessType Type { get; set; }

        void AddNewMember(Character newEmployee);
        bool CanAddNewMember(Character newEmployee);
        bool CanAddNewRank();
        bool CanRemoveRank(BusinessRank rank);
        bool CheckIfRankExists(int rankId);
        bool GetBusinessRank(int rankId, out BusinessRank businessRank);
        bool GetBusinessRankForEmployee(Character employee, out BusinessRank businessRank);
        IEnumerable<Character> GetEmployeesWithRank(int rankId);
        bool IsCharacterEmployee(Character employee);
        bool IsCharacterEmployee(int characterId, out Character employee);
        bool IsWorkingHere(Character character);
        bool RemoveEmployee(Character employee);
        bool RemoveRank(int rankId);
        bool SetDefaultRank(Character employee);
        void SetPosition(Position position);
    }
}