namespace AltVStrefaRPServer.Models.Businesses
{
    public class RestaurantBusiness : Business
    {
        public override int BlipSprite { get; protected set; } = 93;
        public override string BlipName { get; protected set; } = "Restauracja";
        public override int BlipColor { get; protected set; } = 49;
    }
}
