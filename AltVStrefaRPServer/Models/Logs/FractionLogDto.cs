using AltVStrefaRPServer.Models.Fractions;
using System;

namespace AltVStrefaRPServer.Models.Logs
{
    public struct FractionLogDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Money { get; set; }
        public DateTime CreationDate { get; set; }

        public FractionLogDto(Fraction fraction)
        {
            Id = fraction.Id;
            Name = fraction.Name;
            Description = fraction.Description;
            Money = fraction.Money;
            CreationDate = fraction.CreationDate;
        }
    }
}
