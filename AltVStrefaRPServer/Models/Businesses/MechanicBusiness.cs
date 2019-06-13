using System;

namespace AltVStrefaRPServer.Models.Businesses
{
    public class MechanicBusiness : Business
    {
        public override int BlipSprite { get; protected set; } = 78;
        public override string BlipName { get; protected set; } = "Mechanik";
        public override int BlipColor { get; protected set; } = 64;
    }
}
