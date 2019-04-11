namespace AltVStrefaRPServer.Models.Businesses
{
    public class RestaurantBusiness : Business
    {
        public override byte BlipModel { get; protected set; } = 93;
        public override string BlipName { get; protected set; } = "Restauracja";
        public override byte BlipColor { get; protected set; } = 49;
    }
}
