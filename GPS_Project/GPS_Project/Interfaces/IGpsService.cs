using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_Project.Interfaces
{
    public interface IGpsService
    {
        void OpenSettings();
        bool IsGpsEnable();
    }
}
