using DeviceMS.Core.DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeviceMS.Logic.ServiceLayer.Helpers
{
    public class Validator
    {
        public static bool IsValidDevice(Device device)
        {
            if (device == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(device.DeviceName))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(device.Address))
            {
                return false;
            }
            if (device.MaxHourlyCons <= 0)
            {
                return false;
            }
            if (device.Description != null && device.Description.Length > 100)
            {
                return false;
            }

            return true;
        }
    }
}
