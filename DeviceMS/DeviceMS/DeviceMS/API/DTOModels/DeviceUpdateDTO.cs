﻿using DeviceMS.Core.DomainLayer.Models;

namespace DeviceMS.API.DTOModels
{
    public class DeviceUpdateDTO
    {
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
    }
}
