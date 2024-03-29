﻿using AltVStrefaRPServer.Models.Enums;
using System;

namespace AltVStrefaRPServer.Models.Dto
{
    public class BusinessInfoDto
    {
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
        public int OwnerId { get; set; }
        public float Money { get; set; }
        public int MaxMembersCount { get; set; }
        public int Transactions { get; set; }
        public DateTime CreatedAt { get; set; }
        public BusinessType Type { get; set; }
        public int EmployeesCount { get; set; }
        public int Ranks { get; set; }
        public int MaxRanksCount { get; set; }
    }

    public class BusinessEmployeeExtendedDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public int RankId { get; set; }
        public string RankName { get; set; }
    }
}
