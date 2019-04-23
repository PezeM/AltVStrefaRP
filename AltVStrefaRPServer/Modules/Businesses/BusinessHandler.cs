using AltV.Net;
using AltVStrefaRPServer.Models;

namespace AltVStrefaRPServer.Modules.Businesses
{
    public class BusinessHandler
    {
        private BusinessManager _businessManager;

        public BusinessHandler(BusinessManager businessManager)
        {
            _businessManager = businessManager;

            Alt.Log("Intialized business handler.");
        }

        public void OpenBusinessMenu(Character character)
        {

        }
    }
}
