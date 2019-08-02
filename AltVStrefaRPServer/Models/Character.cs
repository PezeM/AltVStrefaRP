using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using System;
using AltVStrefaRPServer.Data;
using AltVStrefaRPServer.Models.Businesses;
using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Models
{
    public class Character : IMoney, IPosition
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public BankAccount BankAccount { get; set; }
        public bool IsBanned {get; set; }
        public bool IsMuted { get; set; }
        public bool CanDriveVehicles { get; set; }
        public PlayerInventoryController PlayerInventory { get; set; }
        public string ProfileImage { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public short Dimension { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public Gender Gender { get; set; }
        public float Money { get; private set; }

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

        public void AddMoney(float amount)
        {
            Money += amount;
            Player?.SetSyncedMetaData(MetaData.PLAYER_MONEY, Money);
        }

        public bool RemoveMoney(float amount)
        {
            if (Money < amount) return false;
            Money -= amount;
            Player?.SetSyncedMetaData(MetaData.PLAYER_MONEY, Money);
            return true;
        }

        public string MoneyTransactionDisplayName() => GetFullName();
    }
}
