using System;
using System.Collections.Generic;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;

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

        public int EmployeesCount => Employees.Count;
        public ICollection<Character> Employees { get; set; }

        public virtual byte BlipModel { get; protected set; }
        public virtual string BlipName { get; protected set; }
        public virtual byte BlipColor { get; protected set; }
        public virtual ushort BlipSprite { get; protected set; }
        public virtual IBlip Blip { get; set; }

        public Position GetPosition()
        {
            return new Position(X,Y,Z);
        }
    }
}
