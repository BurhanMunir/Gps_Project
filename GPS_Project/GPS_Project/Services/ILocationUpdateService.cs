using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_Project.Services {
  
    public interface ILocationUpdateService {
        void GetUserLocation();
        void StopLocationUpdates();
        //bool CheckLocationEnable(bool OpenLocationSetting);
        event EventHandler<ILocationEventArgs> locationObtained;
        void ObtainMyLocation();
    }
    public interface ILocationEventArgs {
     double lat { get; set; }
    double lng { get; set; }
     DateTime TimeStamp { get; set; }
     float Accuracy { get; set; }
     float Speed { get; set; }
    }
}
