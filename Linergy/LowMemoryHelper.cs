using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Phone.Info;

namespace Linergy
{
    //Taken from Nokia Developers page
    //http://www.developer.nokia.com/Community/Wiki/Best_practice_tips_for_delivering_apps_to_Windows_Phone_with_256_MB

    public static class LowMemoryHelper
    {
        public static bool IsLowMemDevice { get; set; }

        static LowMemoryHelper()
        {
            try
            {
                Int64 result = (Int64)DeviceExtendedProperties.GetValue("ApplicationWorkingSetLimit");
                if (result < 94371840L)
                    IsLowMemDevice = true;
                else
                    IsLowMemDevice = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Windows Phone OS update not installed, which indicates a 512-MB device. 
                IsLowMemDevice = false;
            }
        }
    }
}
