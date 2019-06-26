using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;

namespace AltVStrefaRPServer.Services.Businesses
{
    public interface IBusinessService
    {
        bool AddEmployee(Business business, Character newEmployee);
    }
}
