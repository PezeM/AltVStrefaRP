using AltVStrefaRPServer.Models.Enums;
using AltVStrefaRPServer.Models.Fractions;
using AltVStrefaRPServer.Models.Interfaces.Managers;

namespace AltVStrefaRPServer.Services.Money
{
    public class TaxService : ITaxService
    {
        private readonly IFractionsManager _fractionsManager;

        public TaxService(IFractionsManager fractionsManager)
        {
            _fractionsManager = fractionsManager;
        }

        public float CalculateTax(float price, TransactionType transactionType, out TownHallFraction townHall)
        {
            if (!_fractionsManager.TryToGetTownHallFraction(out townHall)) return price;

            switch (transactionType)
            {
                case TransactionType.VehicleSell:
                case TransactionType.VehicleBuy:
                    return townHall.CalculateTax(price, townHall.VehicleTax);
                case TransactionType.BankDeposit:
                case TransactionType.BankWithdraw:
                case TransactionType.BankTransfer:
                    return price;
                case TransactionType.FurnitureBuy:
                case TransactionType.PropertiesBuy:
                case TransactionType.PropertiesSell:
                    return townHall.CalculateTax(price, townHall.PropertyTax);
                case TransactionType.BuyingGuns:
                    return townHall.CalculateTax(price, townHall.GunTax);
                default:
                    return townHall.CalculateTax(price, townHall.GlobalTax);
            }
        }
    }
}
