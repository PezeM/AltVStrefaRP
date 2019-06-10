using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVStrefaRPServer.Services.Fractions;

namespace AltVStrefaRPServer.Models.Fractions
{
    public class Fraction : IMoney, IPosition
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

        public virtual byte BlipModel { get; protected set; }
        public virtual string BlipName { get; protected set; }
        public virtual byte BlipColor { get; protected set; }
        public virtual ushort BlipSprite { get; protected set; }
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

        public Position GetPosition()
        {
            return new Position(X,Y,Z);
        }

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
    }
}
