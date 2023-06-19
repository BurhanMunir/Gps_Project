using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GPS_Project.Droid.Native;
using GPS_Project.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[assembly: Xamarin.Forms.Dependency(typeof(GpsServiceDroid))]
namespace GPS_Project.Droid.Native
{
    public class GpsServiceDroid : IGpsService
    {
        public bool IsGpsEnable()
        {
            LocationManager locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            
            return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
        }
       public void GetCurrentLocation(Intent intent) {
            LocationManager manager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            var provider=manager.IsProviderEnabled(LocationManager.GpsProvider);
            Criteria criteria = new Criteria {
                Accuracy = Accuracy.Fine
            };
           
            string locationProvider = "";
            IList<string> locationproviderList = manager.GetProviders(criteria, true);
            if (locationproviderList.Any()) {
                locationProvider = locationproviderList[0];
            }
           
          
        }
        

        public void OpenSettings()
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings);
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);

            try
            {
                Android.App.Application.Context.StartActivity(intent);

            }
            catch (ActivityNotFoundException activityNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine(activityNotFoundException.Message);
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Error: Gps Activity", Android.Widget.ToastLength.Short).Show();
            }
        }
    }
}