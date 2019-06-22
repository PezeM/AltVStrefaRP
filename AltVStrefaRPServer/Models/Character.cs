using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Fractions;

namespace AltVStrefaRPServer.Models
{
    public class Character : IMoney, IOnMoneyChange, IPosition
    {
        private float _money;

        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public BankAccount BankAccount { get; set; }
        public bool IsBanned {get; set; }
        public bool IsMuted { get; set; }
        public bool CanDriveVehicles { get; set; }

        public string BackgroundImage { get; set; }
        public string ProfileImage { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public short Dimension { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        /// <summary>
        /// 0 is male, 1 is female
        /// </summary>
        public int Gender { get; set; }
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
        public DateTime LastPlayed { get; set; }
        public int TimePlayed { get; set; }

        public IPlayer Player { get; set; }

        public int? CurrentBusinessId { get; set; }
        public Business Business { get; set; }
        public int BusinessRank { get; set; }

        public int? CurrentFractionId { get; set; }
        public Fraction Fraction { get; set; }
        public int FractionRank { get; set; }

        [NotMapped]
        public bool UpdateOnMoneyChange { get; set; } = false;

        public string GetFullName()
        {
            return string.Join(' ', FirstName, LastName);
        }

        public void UpdatePosition(Position newPosition)
        {
            X = newPosition.X;
            Y = newPosition.Y;
            Z = newPosition.Z;
        }

        public Position GetPosition()
        {
            return new Position(X, Y, Z);
        }

        public string MoneyTransactionDisplayName() => GetFullName();

        public void OnMoneyChange()
        {
            Player?.SetSyncedMetaData("money", Money);
        }
    }
}
