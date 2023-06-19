using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using GPS_Project.Models;
using Plugin.Geolocator;
using GPS_Project.Helper;

namespace GPS_Project.Services {
    public class Location {
        bool stopping = false;
        private double previousLatitude;
        private double previousLongitude;
        private double distanceTraveled;
        private DateTimeOffset _startTime;
        private double _secondsSpent;
        public Location() {
        }
        public async void GetLastLocation() {
            var location = await CrossGeolocator.Current.GetPositionAsync();
            _startTime = location.Timestamp;
            previousLatitude = location.Latitude;
            previousLongitude = location.Longitude;
        }
        public async Task Run(CancellationToken token) {
            await Task.Run(async () => {
                while (!stopping) {
                    stopping = token.IsCancellationRequested;
                    try {
                        await Task.Delay((int)(5000));
                        var location = await CrossGeolocator.Current.GetPositionAsync();
                        //if (location != null) {
                        //    if (Helper.Utils.IsDistanceMode) {
                        //        distanceTraveled = Xamarin.Essentials.Location.CalculateDistance(
                        //                        previousLatitude, previousLongitude,
                        //                        .Latitude, e.Position.Longitude,
                        //                        DistanceUnits.Kilometers);
                        //        distanceTraveled = 1000 * distanceTraveled;
                        //        if (distanceTraveled >= Convert.ToDouble(Helper.Utils.Meters)) {
                        //            // Do something with the new GPS coordinates
                        //            Device.BeginInvokeOnMainThread(() => {
                        //                MessagingCenter.Send<object, Plugin.Geolocator.Abstractions.Position>(this, "Location", location);
                        //            });

                        //            previousLatitude = e.Position.Latitude;
                        //            previousLongitude = e.Position.Longitude;
                        //        }
                        //    } else {
                        //        _secondsSpent = (location.Timestamp - _startTime).TotalSeconds;

                        //        if (_secondsSpent >= Helper.Utils.Seconds) {
                        //            _startTime = location.Timestamp;
                        //            Device.BeginInvokeOnMainThread(() => {
                        //                MessagingCenter.Send<object, Plugin.Geolocator.Abstractions.Position>(this, "Location", location);
                        //            });
                        //        }


                        //    }

                        //    //Device.BeginInvokeOnMainThread(() => {
                        //    //    MessagingCenter.Send<object,Plugin.Geolocator.Abstractions.Position>(this, "Location",location);
                        //    //});
                        //}
                    } catch (Exception ex) {
                        Device.BeginInvokeOnMainThread(() => {
                            var errormessage = new LocationErrorMessage();
                            MessagingCenter.Send(errormessage, "LocationError");
                        });
                    }
                }
                return;
            }, token);
        }
    }
}
