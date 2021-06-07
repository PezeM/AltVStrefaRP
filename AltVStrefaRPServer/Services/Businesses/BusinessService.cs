using AltVStrefaRPServer.Database;
using AltVStrefaRPServer.Models;
using AltVStrefaRPServer.Models.Businesses;
using System;

namespace AltVStrefaRPServer.Services.Businesses
{
    public class BusinessService : IBusinessService
    {
        private readonly Func<ServerContext> _factory;
        private readonly IBusinessDatabaseService _businessDatabaseService;

        public BusinessService(Func<ServerContext> factory, IBusinessDatabaseService businessDatabaseService)
        {
            _factory = factory;
            _businessDatabaseService = businessDatabaseService;
        }

        /// <summary>
        /// Adds new employee to business. Fails if can't add new emplyoyee or there is no default rank set
        /// </summary>
        /// <param name="business"></param>
        /// <param name="newEmployee"></param>
        /// <returns></returns>
        public bool AddEmployee(Business business, Character newEmployee)
        {
            if (!business.CanAddNewMember(newEmployee)) return false;
            if (!business.SetDefaultRank(newEmployee)) return false;
            business.AddNewMember(newEmployee);
            return true;
        }
    }
}
