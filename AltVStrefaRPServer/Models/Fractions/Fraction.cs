using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Models.Dto;
using AltVStrefaRPServer.Services.Fractions;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class Fraction : IMoney, IPosition
    {
        private float _money;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public float Money
        {
            get { return _money; }
            set
            {
                _money = value;
                if(UpdateOnMoneyChange) OnMoneyChange();
            }
        }

        public DateTime CreationDate { get; set; }
        public virtual float X { get; set; }
        public virtual float Y { get; set; }
        public virtual float Z { get; set; }

        private List<Character> _employees;
        public IReadOnlyCollection<Character> Employees => _employees;
        public int EmployeesCount => Employees.Count;

        private List<FractionRank> _fractionRanks;
        public IReadOnlyCollection<FractionRank> FractionRanks => _fractionRanks;

        public virtual byte BlipModel { get; protected set; }
        public virtual string BlipName { get; protected set; }
        public virtual byte BlipColor { get; protected set; }
        public virtual ushort BlipSprite { get; protected set; }
        public virtual IBlip Blip { get; set; }

        [NotMapped] 
        public bool UpdateOnMoneyChange => false;

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

        public Position GetPosition()
        {
            return new Position(X,Y,Z);
        }

        public string MoneyTransactionDisplayName() => Name;

        public void OnMoneyChange() {}

        public FractionRankPermissions GetEmployeePermissions(Character employee)
        {
            return _fractionRanks.FirstOrDefault(q => q.Id == employee.FractionRank)?.Permissions;
        }

        public FractionRank GetDefaultRank()
        {
            return _fractionRanks.FirstOrDefault(q => q.IsDefaultRank);
        }

        public FractionRank GetEmployeeRank(Character employee)
        {
            return _fractionRanks.FirstOrDefault(q => q.Id == employee.FractionRank);
        }

        public FractionRank GetRankById(int rankId)
        {
            return _fractionRanks.FirstOrDefault(r => r.Id == rankId);
        }

        public IEnumerable<Character> GetEmployeesWithRank(FractionRank rank)
        {
            return _employees.Where(e => e.FractionRank == rank.Id);
        }

        public void SetEmployeeRank(Character employee, FractionRank defaultRank)
        {
            employee.FractionRank = defaultRank.Id;
        }

        public virtual async Task<bool> RemoveEmployeeAsync(int employeeId, IFractionDatabaseService fractionDatabaseService)
        {
            if (!IsCharacterEmployee(employeeId, out Character employee)) return false;
            if (!CanRemoveEmployee(employee)) return false;

            if (_employees.Remove(employee))
            {
                await fractionDatabaseService.UpdateFractionAsync(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual async Task<bool> AddNewEmployeeAsync(Character newEmployee, IFractionDatabaseService fractionDatabaseService)
        {
            if (!CanAddNewEmployee(newEmployee)) return false;
            _employees.Add(newEmployee);

            var defaultRank = GetDefaultRank();
            if (defaultRank == null) return false;
            newEmployee.FractionRank = defaultRank.Id;

            // Save fraction, need to check if its neede to save employee
            await fractionDatabaseService.UpdateFractionAsync(this);
            return true;
        }

        public async Task<bool> RemoveRankAsync(int rankId, IFractionDatabaseService fractionDatabaseService)
        {
            var rank = GetRankById(rankId);
            if (rank == null) return false;
            if (!CanRemoveRank(rank)) return false;

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

        public async Task<bool> UpdateEmployeeRank(int employeeId, int newRankId, IFractionDatabaseService fractionDatabaseService)
        {
            if (!IsCharacterEmployee(employeeId, out Character employee)) return false;
            if (!CanChangeEmployeeRank(employee)) return false;

            var newRank = GetRankById(newRankId);
            if (newRank == null) return false;

            SetEmployeeRank(employee, newRank);

            await fractionDatabaseService.UpdateFractionAsync(this);

            return true;
        }

        public async Task<bool> AddNewRank(NewFractionRankDto newRank, IFractionDatabaseService fractionDatabaseService)
        {
            if (!CanAddRank(newRank)) return false;

            _fractionRanks.Add(new FractionRank
            {
                RankName = newRank.RankName,
                IsDefaultRank = false,
                IsHighestRank = false,
                Permissions = new FractionRankPermissions
                {
                    CanManageEmployees = newRank.Permissions.CanManageEmployees,
                    CanManageRanks = newRank.Permissions.CanManageRanks,
                    HaveFractionKeys = newRank.Permissions.HaveFractionKeys,
                    CanOpenFractionMenu = newRank.Permissions.CanOpenFractionMenu,
                    HaveVehicleKeys = newRank.Permissions.HaveVehicleKeys,
                }
            });

            await fractionDatabaseService.UpdateFractionAsync(this);
            return true;
        }
        
        public async Task<bool> UpdateRank(int rankId, NewFractionRankDto updatedPermissions, IFractionDatabaseService fractionDatabaseService)
        {
            var rank = GetRankById(rankId);
            if (rank == null) return false;

            rank.RankName = updatedPermissions.RankName;
            rank.Permissions.CanManageEmployees = updatedPermissions.Permissions.CanManageEmployees;
            rank.Permissions.CanManageRanks = updatedPermissions.Permissions.CanManageRanks;
            rank.Permissions.CanOpenFractionMenu = updatedPermissions.Permissions.CanOpenFractionMenu;
            rank.Permissions.HaveFractionKeys = updatedPermissions.Permissions.HaveFractionKeys;
            rank.Permissions.HaveVehicleKeys = updatedPermissions.Permissions.HaveVehicleKeys;

            await fractionDatabaseService.UpdateFractionAsync(this);
            return true;
        }

        protected virtual bool IsCharacterEmployee(int characterId, out Character character)
        {
            character = _employees.FirstOrDefault(q => q.Id == characterId);
            return character != null;
        }

        protected bool CanRemoveEmployee(Character employee)
        {
            var employeeRank = GetEmployeeRank(employee);
            if (employeeRank == null) return false;
            else return !employeeRank.IsHighestRank;
        }

        protected virtual bool CanAddNewEmployee(Character newEmployee)
        {
            if (_employees != null)
            {
                if (newEmployee.Fraction != null || newEmployee.Fraction == this) return false;
                else return true;
            }
            else
            {
                return false;
            }
        }

        private bool CanChangeEmployeeRank(Character employee)
        {
            var employeeRank = GetEmployeeRank(employee);
            if (employeeRank == null) return false;
            else if (employeeRank.IsHighestRank) return false;
            else return true;
        }

        private bool CanRemoveRank(FractionRank rank)
        {
            if (rank.IsHighestRank || rank.IsDefaultRank) return false;
            else return true;
        }

        private bool CanAddRank(NewFractionRankDto newRank)
        {
            return !_fractionRanks.Any(r => r.RankName == newRank.RankName);
        }
    }
}
