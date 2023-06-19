using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Xamarin.Forms;
using GPS_Project.Models;
using GPS_Project.Services;
using Plugin.Geolocator.Abstractions;
using Plugin.Geolocator;
using AndroidX.ConstraintLayout.Core.Motion.Utils;
using Android.Locations;
using GPS_Project.Interfaces;
using Xamarin.Essentials;
using GPS_Project.Helper;
using Location = GPS_Project.Services.Location;
using Java.Util.Concurrent.Locks;
using static Android.OS.PowerManager;
using static Android.Provider.CallLog;
using Shiny.Locations;
using Shiny;
using System.Reactive.Threading.Tasks;

[assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
//Required when targeting Android API 21+
[assembly: UsesFeature("android.hardware.location.gps")]
[assembly: UsesFeature("android.hardware.location.network")]

namespace GPS_Project.Droid.Services {
    [Service(Exported = true, Name = "com.companyname.gps_project.DemoService", Enabled =true)]
    public class AndroidLocationService : Service {
        CancellationTokenSource _cts;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        private double previousLatitude;
        private double previousLongitude;
        private double distanceTraveled;
        private double distanceThreshold = 100; // meters
        private DateTimeOffset _startTime;
        private double _secondsSpent;
        private Handler handler;
        private Action runnable;
        private bool isFirst;
        PowerManager.WakeLock wakeLock;
        static object LOCK = new object();
        readonly IGpsManager manager;
        private bool IsStarted;
        private int DELAY_BETWEEN_LOG_MESSAGES = 5000;
        public AndroidLocationService() {
            this.manager = ShinyHost.Resolve<IGpsManager>();
        }
        public override IBinder OnBind(Intent intent) {
            return null;
        }

        public override  StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId) {
            _cts = new CancellationTokenSource();

            if (IsStarted) {
                //service is already started
            } else {
                Context context = global::Android.App.Application.Context;
                lock (LOCK) {
                    if (wakeLock == null) {
                        var pm = PowerManager.FromContext(context);
                        wakeLock = pm.NewWakeLock(WakeLockFlags.Partial, "Woken lock");
                    }
                }
                Notification notif = DependencyService.Get<INotification>().ReturnNotif();
                InititateLocationProcess();
                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notif);
               
                handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
                IsStarted = true;
                
              
                wakeLock.Acquire();
                

                var l = manager.CurrentListener;
                // this.IsUpdating = l != null;

                var mode = l?.BackgroundMode ?? GpsBackgroundMode.None;
                if (manager.CurrentListener != null) {
                    _= manager.StopListener();
                } else {
                    var access = _= manager.RequestAccess(new GpsRequest {
                        BackgroundMode = GpsBackgroundMode.Realtime
                    });
                    //  this.Access = access.ToString();

                    //if (access != AccessState.Available) {
                    //    //await this.Alert("Insufficient permissions - " + access);
                    //    //return;
                    //}

                    var request = new GpsRequest {
                        BackgroundMode = GpsBackgroundMode.Realtime,
                        Accuracy = GpsAccuracy.Normal,
                    };
                    try {
                        _ = manager.StartListener(request);
                    } catch (Exception ex) {
                        //await this.Alert(ex.ToString());
                    }
                }

            }
            

            return StartCommandResult.Sticky;
        }

        private void CrossGeolocator_Current_PositionError(object sender, PositionErrorEventArgs e) {
            Device.BeginInvokeOnMainThread(() => {
                var errormessage = new LocationErrorMessage();
                MessagingCenter.Send(e.Error.ToString(), "LocationError");
            });
        }
        //This method is called when position changing is listened in Distance Mode and register logs
        private void CrossGeolocator_Current_PositionChanged(object sender, PositionEventArgs e) {
            var location = e.Position;
            if (location != null) {
                if (Helper.Utils.IsDistanceMode) {
                    distanceTraveled = Xamarin.Essentials.Location.CalculateDistance(
                                    previousLatitude, previousLongitude,
                                    e.Position.Latitude, e.Position.Longitude,
                                    DistanceUnits.Kilometers);
                    distanceTraveled= 1000 * distanceTraveled;
                    if (distanceTraveled >=Convert.ToDouble(Helper.Utils.Meters)) {
                        // Do something with the new GPS coordinates
                        Device.BeginInvokeOnMainThread(() => {
                            MessagingCenter.Send<object, Plugin.Geolocator.Abstractions.Position>(this, "Location", location);
                        });

                        previousLatitude = e.Position.Latitude;
                        previousLongitude = e.Position.Longitude;
                    }
                } 
                  //  _secondsSpent = (location.Timestamp - _startTime).TotalSeconds;
                
                    if (_secondsSpent >= Helper.Utils.Seconds) {
                     //   _startTime = location.Timestamp;
                        Device.BeginInvokeOnMainThread(() => {
                            MessagingCenter.Send<object, Plugin.Geolocator.Abstractions.Position>(this, "Location", location);
                        });
                    }
                    

                
            }
            
        }

        public override void OnDestroy() {

            // Stop the handler.
            handler.RemoveCallbacks(runnable);
            // Remove the notification from the status bar.
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(SERVICE_RUNNING_NOTIFICATION_ID);
            Helper.Utils.IsTracking=false;
            //DependencyService.Get<ILocationUpdateService>().StopLocationUpdates();
            //CrossGeolocator.Current.PositionChanged -= CrossGeolocator_Current_PositionChanged;
            IsStarted = false;
            if (_cts != null) {
                _cts.Token.ThrowIfCancellationRequested();
                _cts.Cancel();
              
            }
            if (wakeLock != null) {
                wakeLock.Release();
                wakeLock = null;
            }
            base.OnDestroy();

          
        }
        public async override void OnCreate() {
            _cts = new CancellationTokenSource();
         
            base.OnCreate();
            handler = new Handler(Application.MainLooper);
            runnable = new Action(async () => {
                if (IsStarted) {
                    //await TrackPositionChange();
                    await TrackPositionShiny(LocationRetrieve.Current);
                    handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
                   
                }
            });
        }
        private async Task TrackPositionShiny(LocationRetrieve retrieve) {
            var observable = retrieve switch {
                LocationRetrieve.Current => this.manager.GetCurrentPosition()
            };
            var reading = await observable.ToTask();
            if(reading != null) {

            }
        }
        enum LocationRetrieve {
            Last,
            Current,
            LastOrCurrent
        }
        private async Task TrackPositionChange() {
           
                try {
                   // var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                   var location = await Geolocation.GetLocationAsync();
                // var location = await CrossGeolocator.Current.GetLastKnownLocationAsync();
                //var location = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(10), null, true);
              
                    if (Helper.Utils.IsDistanceMode) {
                        distanceTraveled = Xamarin.Essentials.Location.CalculateDistance(
                                        previousLatitude, previousLongitude,
                                        location.Latitude, location.Longitude,
                                        DistanceUnits.Kilometers);
                        distanceTraveled = 1000 * distanceTraveled;
                        if (distanceTraveled >= Convert.ToDouble(Helper.Utils.Meters)) {
                            Device.BeginInvokeOnMainThread(() => {
                                MessagingCenter.Send<object, Xamarin.Essentials.Location>(this, "LocationAndroid", location);
                            });

                            previousLatitude = location.Latitude;
                            previousLongitude = location.Longitude;
                        }
                    }

                    _secondsSpent = (location.Timestamp - _startTime).TotalSeconds;

                    if (_secondsSpent >= Helper.Utils.Seconds) {
                       _startTime = location.Timestamp;
                        Device.BeginInvokeOnMainThread(() => {
                            MessagingCenter.Send<object, Xamarin.Essentials.Location>(this, "LocationAndroid", location);
                        });
                    }
                } catch (Exception ex) {
                    Device.BeginInvokeOnMainThread(async () => {
                        await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
                    });
                }
    
           
        }
        //GpsBackgroundMode GetMode() {
        //    var mode = GpsBackgroundMode.None;
        //    if (this.UseBackground) {
        //        mode = this.UseRealtime
        //            ? GpsBackgroundMode.Realtime
        //            : GpsBackgroundMode.Standard;
        //    }
        //    return mode;
        //}
        private void AndroidLocationService_locationObtained(object sender, ILocationEventArgs e) {
             var location = e;
            if(location!=null) {
                GpsModel gps = new GpsModel() {
                    Latitude=location.lat.ToString(),
                    Longitude=location.lng.ToString(),
                    TimeStamp=location.TimeStamp.ToString()

                };
                if (isFirst) {
                    previousLatitude = e.lat;
                    previousLongitude= e.lng;
                    _startTime = e.TimeStamp;
                    isFirst = false;
                } else {
                    if (Helper.Utils.IsDistanceMode) {
                        distanceTraveled = Xamarin.Essentials.Location.CalculateDistance(
                                        previousLatitude, previousLongitude,
                                        location.lat, location.lng,
                                        DistanceUnits.Kilometers);
                        distanceTraveled = 1000 * distanceTraveled;
                        if (distanceTraveled >= Convert.ToDouble(Helper.Utils.Meters)) {
                            Device.BeginInvokeOnMainThread(() => {
                                MessagingCenter.Send<object, GpsModel>(this, "LocationAndroidTest", gps);
                            });

                            previousLatitude = location.lat;
                            previousLongitude = location.lng;
                        }
                    }

                    _secondsSpent = (location.TimeStamp - _startTime).TotalSeconds;
                
                    if (_secondsSpent >= Helper.Utils.Seconds) {
                        _startTime = location.TimeStamp;
                        Device.BeginInvokeOnMainThread(() => {
                            MessagingCenter.Send<object, GpsModel>(this, "LocationAndroidTest", gps);
                        });
                    }
                }
            }
        }

        private async void InititateLocationProcess() {


            var status = await CheckAndRequestLocationPermission();
            if (status == PermissionStatus.Granted) {
                var location = await Geolocation.GetLocationAsync();
                if (location != null) {
                    previousLatitude = location.Latitude;
                    previousLongitude = location.Longitude;
                  _startTime = location.Timestamp;
                } 
            }
        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission() {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            try {

                if (status == PermissionStatus.Granted)
                    return status;

                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS) {
                    // Prompt the user to turn on in settings
                    // On iOS once a permission has been denied it may not be requested again from the application
                    return status;
                }

                if (Permissions.ShouldShowRationale<Permissions.LocationAlways>()) {
                    // Prompt the user with additional information as to why the permission is needed
                }
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                status = await Permissions.RequestAsync<Permissions.LocationAlways>();

            } catch (Exception ex) {

            }
            return status;
        }
    }
}