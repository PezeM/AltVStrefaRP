﻿using AltVStrefaRPServer.Models.Inventory;

namespace AltVStrefaRPServer.Models.Interfaces.Inventory
{
    public interface IEquipmentable
    {
        EquipmentSlot Slot { get; set; }
    }
}