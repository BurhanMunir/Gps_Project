using System;
using System.Threading;
using System.Threading.Tasks;
using GPS_Project.Models;
using GPS_Project.Services;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GPS_Project.iOS.Services {
    public class iOsLocationService {
        private double previousLatitude;
        private double previousLongitude;
        private double distanceTraveled;
        private double distanceThreshold = 100; // meters
        private DateTimeOffset _startTime;
        private double _secondsSpent;

        nint _taskId;
        CancellationTokenSource _cts;
        public bool isStarted = false;

        public async Task Start() {
            _cts = new CancellationTokenSource();
            _taskId = UIApplication.SharedApplication.BeginBackgroundTask("com.companyname.gpsproject", OnExpiration);

            try {
                /*var locShared = new Location();
				isStarted = true;
				await locShared.Run(_cts.Token);*/
                if (Helper.Utils.IsTracking) {
                    CrossGeolocator.Current.PositionChanged -= CrossGeolocator_Current_PositionChanged;
                    CrossGeolocator.Current.PositionError -= CrossGeolocator_Current_PositionError;
                } else {
                    CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                    CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;
                }
                if (CrossGeolocator.Current.IsListening) {
                    await CrossGeolocator.Current.StopListeningAsync();
                    Helper.Utils.IsTracking = false;

                } else {
                    if (Helper.Utils.IsDistanceMode) {
                        if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(0), 0, true, new ListenerSettings {
                            AllowBackgroundUpdates = true,
                            DeferLocationUpdates = false,
                            DeferralDistanceMeters = null,
                            DeferralTime = TimeSpan.FromSeconds(4),
                            ListenForSignificantChanges = false,
                            PauseLocationUpdatesAutomatically = true

                        })) {
                            Helper.Utils.IsTracking = true;
                        }
                    } else {
                        if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(0), 0, true, new ListenerSettings {
                            AllowBackgroundUpdates = true,
                            DeferLocationUpdates = false,
                            DeferralDistanceMeters = null,
                            DeferralTime = TimeSpan.FromSeconds(2),
                            ListenForSignificantChanges = false,
                            PauseLocationUpdatesAutomatically = true

                        })) {
                            Helper.Utils.IsTracking = true;
                        }

                    }





                }
            } catch (OperationCanceledException) {
            } finally {
                if (_cts.IsCancellationRequested) {
                    var message = new StopServiceMessage();
                    Device.BeginInvokeOnMainThread(
                        () => MessagingCenter.Send(message, "ServiceStopped")
                    );
                }
            }

            var time = UIApplication.SharedApplication.BackgroundTimeRemaining;

            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
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
                    distanceTraveled = 1000 * distanceTraveled;
                    if (distanceTraveled >= Convert.ToDouble(Helper.Utils.Meters)) {
                        // Do something with the new GPS coordinates
                        Device.BeginInvokeOnMainThread(() => {
                            MessagingCenter.Send<object, Plugin.Geolocator.Abstractions.Position>(this, "Location", location);
                        });

                        previousLatitude = e.Position.Latitude;
                        previousLongitude = e.Position.Longitude;
                    }
                }
                _secondsSpent = (location.Timestamp - _startTime).TotalSeconds;

                if (_secondsSpent >= Helper.Utils.Seconds) {
                    _startTime = location.Timestamp;
                    Device.BeginInvokeOnMainThread(() => {
                        MessagingCenter.Send<object, Plugin.Geolocator.Abstractions.Position>(this, "Location", location);
                    });
                }



            }

        }

        public void Stop() {
            isStarted = false;
            _cts.Cancel();
            if (CrossGeolocator.Current.IsListening) {
                CrossGeolocator.Current.StopListeningAsync();
                //	UIApplication.SharedApplication.EndBackgroundTask(_taskId);
                Helper.Utils.IsTracking = false;
            }
        }

        void OnExpiration() {
            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }
    }
}
