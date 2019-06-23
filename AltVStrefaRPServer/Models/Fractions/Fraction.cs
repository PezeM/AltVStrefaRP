using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions.Permissions;
using AltVStrefaRPServer.Services.Fractions;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class Fraction : IMoney, IPosition, IHaveBlip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public float Money { get; set; }

        public DateTime CreationDate { get; set; }
        public virtual float X { get; set; }
        public virtual float Y { get; set; }
        public virtual float Z { get; set; }

        private List<Character> _employees;
        public IReadOnlyCollection<Character> Employees => _employees;
        public int EmployeesCount => Employees.Count;

        private List<FractionRank> _fractionRanks;
        public IReadOnlyCollection<FractionRank> FractionRanks => _fractionRanks;

        public virtual string BlipName { get; protected set; }
        public virtual int BlipColor { get; protected set; }
        public virtual int BlipSprite { get; protected set; }
        public virtual IBlip Blip { get; set; }

        protected Fraction()
        {
            _employees = new List<Character>();
            _fractionRanks = new List<FractionRank>();
        }

        public Fraction(string name, string description, float money, Position position)
        {
            Name = name;
            Description = description;
            Money = money;
            X = position.X;
            Y = position.Y;
            Z = position.Z;

            _fractionRanks = new List<FractionRank>();
            _employees = new List<Character>();
        }

        public Position GetPosition() => new Position(X, Y, Z);

        public string MoneyTransactionDisplayName() => Name;

        public bool HasPermission<TPermission>(Character character) where TPermission : FractionPermission 
            => GetEmployeeRank(character)?.HasPermission<TPermission>() ?? false;

        public FractionPermission GetPermission<TPermission>(Character character) where TPermission : FractionPermission 
            => GetEmployeeRank(character)?.GetPermission<TPermission>();

        public ICollection<FractionPermission> GetEmployeePermissions(Character employee) 
            => _fractionRanks.FirstOrDefault(q => q.Id == employee.FractionRank)?.Permissions;

        public FractionRank GetDefaultRank() => _fractionRanks.FirstOrDefault(r => r.RankType == RankType.Default);

        public FractionRank GetHighestRank() => _fractionRanks.FirstOrDefault(r => r.RankType == RankType.Highest);

        public FractionRank GetEmployeeRank(Character employee) => _fractionRanks.FirstOrDefault(q => q.Id == employee.FractionRank);

        public FractionRank GetRankById(int rankId) => _fractionRanks.FirstOrDefault(r => r.Id == rankId);

        public IEnumerable<Character> GetEmployeesWithRank(FractionRank rank) => _employees.Where(e => e.FractionRank == rank.Id);

        public Character GetOwner()
        {
            var highestRank = GetHighestRank();
            if (highestRank == null) return null;
            return Employees.FirstOrDefault(e => e.FractionRank == highestRank.Id);
        }

        public void SetEmployeeRank(Character employee, FractionRank defaultRank)
        {
            employee.FractionRank = defaultRank.Id;
        }

        public virtual async Task<bool> RemoveEmployeeAsync(Character characterRemoving, int employeeId, IFractionDatabaseService fractionDatabaseService)
        {
            if (!IsCharacterEmployee(employeeId, out Character employee)) return false;
            if (!CanRemoveEmployee(characterRemoving, employee)) return false;
            if (!_employees.Remove(employee)) return false;

            employee.CurrentFractionId = null;
            await fractionDatabaseService.UpdateFractionAsync(this);
            return true;
        }

        public virtual async Task<bool> AddNewEmployeeAsync(Character newEmployee, IFractionDatabaseService fractionDatabaseService)
        {
            if (!CanAddNewEmployee(newEmployee)) return false;
            _employees.Add(newEmployee);

            var defaultRank = GetDefaultRank();
            if (defaultRank == null) return false;
            SetEmployeeRank(newEmployee, defaultRank);

            // Save fraction, need to check if its neede to save employee
            await fractionDatabaseService.UpdateFractionAsync(this);
            return true;
        }

        public async Task<bool> RemoveRankAsync(int rankId, IFractionDatabaseService fractionDatabaseService)
        {
            var rank = GetRankById(rankId);
            if (rank == null) return false;
            //if (!CanRemoveRank(rank)) return false;

            if (_fractionRanks.Remove(rank))
            {
                var employeesWithRank = GetEmployeesWithRank(rank);
                var defaultRank = GetDefaultRank();
                if (employeesWithRank.Count() > 0)
                {
                    foreach (var employee in employeesWithRank)
                    {
                        SetEmployeeRank(employee, defaultRank);
                    }
                }

                await fractionDatabaseService.UpdateFractionAsync(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeRank(Character employeeChangingRank, int employeeId, int newRankId, IFractionDatabaseService fractionDatabaseService)
        {
            if (!IsCharacterEmployee(employeeId, out Character employee)) return false;
            var newRank = GetRankById(newRankId);
            if (newRank == null) return false;
            if (!CanChangeEmployeeRank(employeeChangingRank, employee, newRank)) return false;

            if (newRank.RankType == RankType.Highest)
            {
                return await SetFractionOwner(employee, fractionDatabaseService);
            }
            else
            {
                SetEmployeeRank(employee, newRank);
                await fractionDatabaseService.UpdateFractionAsync(this);
                return true;
            }
        }

        public async Task<bool> AddNewRank(NewFractionRankDto newRank, IFractionDatabaseService fractionDatabaseService)
        {
            if (!CanAddRank(newRank)) return false;

            _fractionRanks.Add(new FractionRank
            {
                RankName = newRank.RankName,
                RankType = RankType.Normal,
                Permissions = new List<FractionPermission>
                {
                    new OpenMenuPermission(false),
                    new VehiclePermission(false),
                }
            });

            await fractionDatabaseService.UpdateFractionAsync(this);
            return true;
        }
        
        public async Task<bool> UpdateRank(int rankId, NewFractionRankDto updatedPermissions, IFractionDatabaseService fractionDatabaseService)
        {
            var rank = GetRankById(rankId);
            if (rank == null) return false;


            await fractionDatabaseService.UpdateFractionAsync(this);
            return true;
        }

        public async Task<bool> SetFractionOwner(Character newOwner, IFractionDatabaseService fractionDatabaseService)
        {
            if ((newOwner.CurrentFractionId ?? 0) != Id) return false;
            var highestRank = GetHighestRank();
            if (highestRank == null) return false;
            var defaultRank = GetDefaultRank();
            if (defaultRank == null) return false;
            var oldOwner = GetOwner();
            if (oldOwner != null)
            {
                SetEmployeeRank(oldOwner, defaultRank);
            }

            SetEmployeeRank(newOwner, highestRank);
            await fractionDatabaseService.UpdateFractionAsync(this);
            return true;
        }

        protected virtual bool IsCharacterEmployee(int characterId, out Character character)
        {
            character = _employees.FirstOrDefault(q => q.Id == characterId);
            return character != null;
        }

        protected bool CanRemoveEmployee(Character characterRemoving, Character employee)
        {
            var employeeRank = GetEmployeeRank(employee);
            if (employeeRank == null) return false;
            var characterRemovingRank = GetEmployeeRank(characterRemoving);
            if (characterRemovingRank == null) return false;
            else if (!characterRemovingRank.HasHigherPriority(employeeRank)) return false;
            else return !(employeeRank.RankType == RankType.Highest);
        }

        protected virtual bool CanAddNewEmployee(Character newEmployee)
        {
            if (_employees != null)
            {
                if ((newEmployee.CurrentFractionId ?? 0) == Id) return false;
                else return true;
            }
            else
            {
                return false;
            }
        }

        private bool CanChangeEmployeeRank(Character employeeModifying, Character employee, FractionRank newRank)
        {
            var employeeRank = GetEmployeeRank(employee);
            if (employeeRank == null) return false;
            if (newRank.Id == employeeRank.Id) return false;
            var employeeModifyingRank = GetEmployeeRank(employeeModifying);
            if (employeeModifyingRank == null) return false;

            return employeeModifyingRank.HasHigherPriority(newRank) && employeeModifyingRank.HasHigherPriority(employeeRank);
        }

        private bool CanRemoveRank(FractionRank removerRank, FractionRank rankToRemove)
        {
            if (rankToRemove.RankType == RankType.Default || rankToRemove.RankType == RankType.Highest) return false;
            else if (removerRank.Priority <= rankToRemove.Priority) return false;
            else return true;
        }

        private bool CanAddRank(NewFractionRankDto newRank)
        {
            // Cant be the same name,
            // cant be default or highest,
            // cant have the same priorty
            if (!_fractionRanks.Any(r => r.RankName == newRank.RankName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
