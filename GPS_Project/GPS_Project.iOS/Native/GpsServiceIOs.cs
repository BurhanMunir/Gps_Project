using CoreLocation;
using Foundation;
using GPS_Project.Interfaces;
using GPS_Project.iOS.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
[assembly: Xamarin.Forms.Dependency(typeof(GpsServiceIOs))]
namespace GPS_Project.iOS.Native
{
    public class GpsServiceIOs : IGpsService
    {
        public bool IsGpsEnable()
        {
            if (CLLocationManager.Status == CLAuthorizationStatus.Denied)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void OpenSettings()
        {
            var WiFiURL = new NSUrl("prefs:root=WIFI");

            if (UIApplication.SharedApplication.CanOpenUrl(WiFiURL))
            {   //> Pre iOS 10
                UIApplication.SharedApplication.OpenUrl(WiFiURL);
            }
            else
            {   //> iOS 10
                UIApplication.SharedApplication.OpenUrl(new NSUrl("App-Prefs:root=WIFI"));
            }
        }
    }
}