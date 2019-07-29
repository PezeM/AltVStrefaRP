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

        public float CalculatePriceAfterTax(float price, TransactionType transactionType)
        {
            if (!_fractionsManager.TryToGetTownHallFraction(out TownHallFraction townHall)) return price;

            switch (transactionType)
            {
                case TransactionType.VehicleSell: case TransactionType.VehicleBuy:
                    return townHall.PriceAfterTax(price, townHall.VehicleTax);
                case TransactionType.BankDeposit: case TransactionType.BankWithdraw: case TransactionType.BankTransfer:
                    return price;
                case TransactionType.BuyingFurnitures: case TransactionType.BuyingProperties:
                    return townHall.PriceAfterTax(price, townHall.PropertyTax);
                case TransactionType.BuyingGuns:
                    return townHall.PriceAfterTax(price, townHall.GunTax);
                default:
                    return townHall.PriceAfterTax(price, townHall.GlobalTax);
            }
        }
    }
}
