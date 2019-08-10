using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Models.Dto.Fractions;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using AltVStrefaRPServer.Models.Interfaces;
using AltVStrefaRPServer.Services.Fractions;

namespace AltVStrefaRPServer.Models.Fractions
{
    public interface IFraction : IMoney, IPosition, IHaveBlip
    {
        IBlip Blip { get; set; }
        DateTime CreationDate { get; set; }
        string Description { get; set; }
        IReadOnlyCollection<Character> Employees { get; }
        int EmployeesCount { get; }
        IReadOnlyCollection<FractionRank> FractionRanks { get; }
        int Id { get; set; }
        List<int> Invites { get; }
        string Name { get; set; }

        Task<bool> AddNewEmployeeAsync(Character newEmployee, IFractionDatabaseService fractionDatabaseService);
        Task<bool> AddNewRankAsync(NewFractionRankDto newRank, IFractionDatabaseService fractionDatabaseService);
        bool CancelFractionInvite(Character employee);
        Task<bool> ForceFractionOwnerAsync(Character newOwner, IFractionDatabaseService fractionDatabaseService);
        FractionRank GetDefaultRank();
        ICollection<FractionPermission> GetEmployeePermissions(Character employee);
        FractionRank GetEmployeeRank(Character employee);
        IEnumerable<Character> GetEmployeesWithRank(FractionRank rank);
        FractionRank GetHighestRank();
        Character GetOwner();
        FractionPermission GetPermission<TPermission>(Character character) where TPermission : FractionPermission;
        FractionRank GetRankById(int rankId);
        bool HasPermission<TPermission>(Character character) where TPermission : FractionPermission;
        Task<bool> RemoveEmployeeAsync(Character characterRemoving, int employeeId, IFractionDatabaseService fractionDatabaseService);
        Task<bool> RemoveRankAsync(Character remover, int rankId, IFractionDatabaseService fractionDatabaseService);
        bool SendInviteToFraction(Character newEmployee);
        void SetEmployeeRank(Character employee, FractionRank defaultRank);
        Task<bool> SetFractionOwnerAsync(Character newOwner, IFractionDatabaseService fractionDatabaseService);
        Task<bool> UpdateEmployeeRankAsync(Character employeeChangingRank, int employeeId, int newRankId, IFractionDatabaseService fractionDatabaseService);
        Task<bool> UpdateRankAsync(Character updatingEmployee, UpdatedFractionRankDto updatedRank, IFractionDatabaseService fractionDatabaseService);
    }
}