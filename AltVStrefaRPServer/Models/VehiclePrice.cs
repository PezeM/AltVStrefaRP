namespace AltVStrefaRPServer.Models
{
    public class VehiclePrice
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public AltV.Net.Enums.VehicleModel VehicleModel { get; set; }

        private VehiclePrice(){}

        public VehiclePrice(int price, AltV.Net.Enums.VehicleModel vehicleModel)
        {
            Price = price;
            VehicleModel = vehicleModel;
        }
    }
}
