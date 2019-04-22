using System;

namespace AltVStrefaRPServer.Models.Businesses
{
    public class MechanicBusiness : Business
    {
        public override byte BlipModel { get; protected set; } = 78;
        public override string BlipName { get; protected set; } = "Mechanik";
        public override byte BlipColor { get; protected set; } = 64;
    }
}
