using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GPS_Project.Droid.Native;
using GPS_Project.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
[assembly: Xamarin.Forms.Dependency(typeof(LocationService))]
namespace GPS_Project.Droid.Native {
    //---event arguments containing lat and lng---
    public class LocationEventArgs : EventArgs, ILocationEventArgs {
        public double lat { get; set; }
        public double lng { get; set; }
        public DateTime TimeStamp { get; set; }
        public float Accuracy { get; set; }
        public float Speed { get; set; }
    }

    public class LocationService : Java.Lang.Object,
                                GPS_Project.Services.ILocationUpdateService,
                                ILocationListener {
        public LocationService() {

        }
        LocationManager lm;
        public void OnProviderDisabled(string provider) { }
        public void OnProviderEnabled(string provider) { }
        public void OnStatusChanged(string provider,
            Availability status, Android.OS.Bundle extras) { }
        //---fired whenever there is a change in location---
        public void OnLocationChanged(Android.Locations.Location location) {
            if (location != null) {
                LocationEventArgs args = new LocationEventArgs();
                args.lat = location.Latitude;
                args.lng = location.Longitude;
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime date = start.AddMilliseconds(location.Time).ToLocalTime();
                args.TimeStamp = date;
                locationObtained(this, args);
                
            };
        }
        //---an EventHandler delegate that is called when a location
        // is obtained---
        public event EventHandler<ILocationEventArgs>
            locationObtained;
        public event EventHandler<ILocationEventArgs> LocationChanged;

        //---custom event accessor that is invoked when client
        // subscribes to the event---
        event EventHandler<ILocationEventArgs>
            GPS_Project.Services.ILocationUpdateService.locationObtained {
            add {
                locationObtained += value;
            }
            remove {
                locationObtained -= value;
            }
        }
        //---method to call to start getting location---
        public void ObtainMyLocation() {
            lm = (LocationManager)
                Forms.Context.GetSystemService(
                    Context.LocationService);
            lm.RequestLocationUpdates(
                LocationManager.NetworkProvider,
                    0,   //---time in ms---
                    0,   //---distance in metres---
                    this);
        }

        public void GetUserLocation() {
            throw new NotImplementedException();
        }

        public void StopLocationUpdates() {
            lm.RemoveUpdates(this);
        }

        //public void OnLocationChanged(Android.Locations.Location location) {
        //    throw new NotImplementedException();
        //}

        //---stop the location update when the object is set to
        // null---
        //LocationService() {
            
        //}
    }
}