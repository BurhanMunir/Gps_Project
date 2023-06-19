using GPS_Project.Models;
using GPS_Project.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using GPS_Project.Interfaces;
using GPS_Project.Helper;
using System.Timers;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System.Drawing;
using Shiny.Locations;
using Shiny;
using System.Runtime.CompilerServices;

namespace GPS_Project.ViewModels {
    public class MainViewModel :  Shiny.NotifyPropertyChanged ,INotifyPropertyChanged{

        #region Commands
        public ICommand LogsPageCommand { get; set; }
        public ICommand GpsCommand { get; set; }
        #endregion

        #region Properties
        public INavigation Navigation { get; set; }
        public TimerCallback LocationCheckTimer;
        readonly IGpsManager manager;

        public System.Timers.Timer aTimer;
        string _gpsStatus;
        public string GpsStatus {
            get => _gpsStatus;
            set {
                _gpsStatus = value;
                OnPropertyChanged();
            }
        }
        bool _tracking;
        public bool Tracking {
            get { return _tracking; }
            set {
                _tracking = value;
                OnPropertyChanged();
            }
        }
        GpsModel _gpsObject;
        public GpsModel GpsObject {
            get { return _gpsObject; }
            set { _gpsObject = value; OnPropertyChanged(); }
        }
        string _latitude = "0.00";
        public string Latitude {
            get => _latitude;
            set {
                _latitude = value;
                OnPropertyChanged();
            }
        }
        string _longitude = "0.00";
        public string Longitude {
            get => _longitude;
            set {
                _longitude = value;
                OnPropertyChanged();
            }
        }
        string _heading = "0.00";
        public string Heading {
            get => _heading;
            set {
                _heading = value;
                OnPropertyChanged();
            }
        }
        string _headingAccuracy = "0.00";
        public string HeadingAccuracy {
            get => _headingAccuracy;
            set {
                _headingAccuracy = value;
                OnPropertyChanged();
            }
        }

        string _positionAccuracy = "0.00";
        public string PositionAccuracy {
            get => _positionAccuracy;
            set {
                _positionAccuracy = value;
                OnPropertyChanged();
            }
        }
        string _speed = "0.00";
        public string Speed {
            get => _speed;
            set {
                _speed = value;
                OnPropertyChanged();
            }
        }

        string _timestamp = DateTime.Now.ToString();
        public string TimeStamp {
            get => _timestamp;
            set {
                _timestamp = value;
                OnPropertyChanged();
            }
        }
        string _lastTimestamp;
        public string LastTimeStamp {
            get => _lastTimestamp;
            set {
                _lastTimestamp = value;
                OnPropertyChanged();
            }
        }
        string _accuracy;
        public string Accuracy {
            get => _accuracy;
            set {
                _accuracy = value;
                OnPropertyChanged();
            }
        }
        int _meters = 10;
        public int Meters {
            get => _meters;
            set {
                _meters = value;
                OnPropertyChanged();
            }
        }
        int _logsCount;
        public int LogCount {
            get => _logsCount; set {
                _logsCount = value;
                OnPropertyChanged();
            }
        }
        int _seconds = 10;
        public int Seconds {
            get => _seconds;
            set {
                _seconds = value;
                Utils.Seconds = value;
                OnPropertyChanged();
            }
        }
        string _appStatusText = "GPS Deactivated";
        public string AppStatusText {
            get => _appStatusText;
            set {
                _appStatusText = value;
                OnPropertyChanged();
            }
        }

        string _gpsButtonText = "START GPS";
        public string GpsButtonText {
            get => _gpsButtonText;
            set {
                _gpsButtonText = value;
                OnPropertyChanged();
            }
        }
        string _selectedAccuracy = "Default";
        bool _isDistanceMode;
        public bool IsDistanceMode {
            get => _isDistanceMode;
            set {
                _isDistanceMode = value;
                OnPropertyChanged();
            }
        }
        public string SelectedAccuracy {
            get => _selectedAccuracy;
            set {
                _selectedAccuracy = value;
                switch (value) {
                    case "Best":
                        this.GeolocationAccuracy = GeolocationAccuracy.Best; break;
                    case "High":
                        this.GeolocationAccuracy = GeolocationAccuracy.High; break;
                    case "Medium":
                        this.GeolocationAccuracy = GeolocationAccuracy.Medium; break;

                    case "Low":
                        this.GeolocationAccuracy = GeolocationAccuracy.Low; break;
                    case "Lowest":
                        this.GeolocationAccuracy = GeolocationAccuracy.Lowest; break;
                    default:
                        this.GeolocationAccuracy = GeolocationAccuracy.Default; break;

                }
                OnPropertyChanged();
            }
        }
        GeolocationAccuracy _geolocationAccuracy;
        public GeolocationAccuracy GeolocationAccuracy {
            get => _geolocationAccuracy;
            set {
                _geolocationAccuracy = value;
                OnPropertyChanged();
            }
        }
        bool _startEnabled;
        public bool StartEnabled {
            get => _startEnabled;
            set {
                _startEnabled = value;
                OnPropertyChanged();
            }
        }
        bool _stopEnabled;
        public bool StopEnabled {
            get => _stopEnabled;
            set {
                _stopEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public MainViewModel(INavigation navigation) {
            Navigation = navigation;
            GetCount();
            CheckGps();
            _ = CheckAndRequestLocationPermission();
            manager = ShinyHost.Resolve<IGpsManager>();
            HandleReceivedMessages();
            LogsPageCommand = new Command(async () => {
                await Navigation.PushAsync(new LogsView());
            });
            GpsCommand = new Command(async (arg) => {
                var status = await CheckAndRequestLocationPermission();
                Utils.Meters = Meters;
                Utils.Seconds = Convert.ToDouble(Seconds);
                Utils.GeolocationAccuracy = this.GeolocationAccuracy;

                if (IsDistanceMode) {
                    if ((string)arg == "START GPS") {
                        Utils.IsDistanceMode = true;
                        //  TrackInDistance();
                        await OnStartClick();
                        GpsButtonText = "STOP GPS";
                        IsBusy = false;
                    } else if ((string)arg == "STOP GPS") {
                        //TrackInDistance();
                        await OnStopClick();
                        GpsButtonText = "START GPS";
                        IsBusy = true;
                        AppStatusText = "GPS Deactivated";
                    }
                } else {

                    if ((string)arg == "START GPS") {
                        //SetTimer();
                        Utils.IsDistanceMode = false;
                        await OnStartClick();
                        // TrackInTime();
                        GpsButtonText = "STOP GPS";
                        IsBusy = false;
                    } else if ((string)arg == "STOP GPS") {
                        await OnStopClick();
                        //StopTimer();
                        //TrackInTime();
                        GpsButtonText = "START GPS";
                        IsBusy = true;
                        AppStatusText = "GPS Deactivated";
                    }
                }
            });

        }
        #endregion

        #region Methods
        public bool CheckGps() {
            if (DependencyService.Get<IGpsService>().IsGpsEnable()) {
                GpsStatus = "Active";
                return true;

            } else {
                GpsStatus = "Deactive";
                return false;
            }
        }
        private void SetTimer() {
            Task.Run(() => {
                // Create a timer with a five second interval.
                aTimer = new System.Timers.Timer(Seconds * 1000);
                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += OnTimedEventAsync;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
            });


        }
        private void StopTimer() {
            aTimer.Elapsed -= OnTimedEventAsync;
            aTimer.AutoReset = false;
            aTimer.Enabled = false;

        }

        //GetLocation When DistanceMode is on
        public async void TrackInDistance() {
            var status = await CheckAndRequestLocationPermission();
            if (Tracking) {
                CrossGeolocator.Current.PositionChanged -= CrossGeolocator_Current_PositionChanged;
                CrossGeolocator.Current.PositionError -= CrossGeolocator_Current_PositionError;
            } else {
                CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;
            }
            if (CrossGeolocator.Current.IsListening) {
                await CrossGeolocator.Current.StopListeningAsync();
                Tracking = false;
                AppStatusText = "GPS Deactivated";
            } else {

                if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), Meters, false, new ListenerSettings {
                    AllowBackgroundUpdates = true,
                    DeferLocationUpdates = false,
                    DeferralDistanceMeters = Meters,
                    DeferralTime = null,
                    ListenForSignificantChanges = false,
                    PauseLocationUpdatesAutomatically = true

                })) {
                    Tracking = true;
                }
            }



        }

        public async void TrackInTime() {
            if (Tracking) {
                CrossGeolocator.Current.PositionChanged -= CrossGeolocator_Current_PositionChanged;
                CrossGeolocator.Current.PositionError -= CrossGeolocator_Current_PositionError;
                // StopTimer();
            } else {


                CrossGeolocator.Current.PositionChanged += CrossGeolocator_Current_PositionChanged;
                CrossGeolocator.Current.PositionError += CrossGeolocator_Current_PositionError;
                //SetTimer();
            }
            if (CrossGeolocator.Current.IsListening) {
                await CrossGeolocator.Current.StopListeningAsync();
                Tracking = false;
                AppStatusText = "GPS Deactivated";
            } else {

                if (await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(1), 0.5, false, new ListenerSettings {
                    AllowBackgroundUpdates = true,
                    DeferLocationUpdates = true,
                    DeferralDistanceMeters = null,
                    DeferralTime = TimeSpan.FromSeconds(Seconds),
                    ListenForSignificantChanges = true,
                    PauseLocationUpdatesAutomatically = false

                })) {
                    Tracking = true;
                }
            }

        }

        private void CrossGeolocator_Current_PositionError(object sender, PositionErrorEventArgs e) {
            AppStatusText = e.Error.ToString();
        }
        //This method is called when position changing is listened in Distance Mode and register logs
        private void CrossGeolocator_Current_PositionChanged(object sender, PositionEventArgs e) {
            Device.BeginInvokeOnMainThread(async () => {
                PermissionStatus permissionStatus = await CheckAndRequestLocationPermission();
                try {

                    if (permissionStatus == PermissionStatus.Granted) {
                        if (DependencyService.Get<IGpsService>().IsGpsEnable()) {
                            if (Connectivity.NetworkAccess == NetworkAccess.Internet) {
                                AppStatusText = "Connecting to GPS";
                                var lastlocation = e.Position;
                                if (lastlocation != null) {
                                    GpsModel gpsModel = new GpsModel();
                                    Latitude = lastlocation.Latitude.ToString();
                                    gpsModel.Latitude = lastlocation.Latitude.ToString().Split('.')[0];
                                    Longitude = lastlocation.Longitude.ToString();
                                    gpsModel.Longitude = lastlocation.Longitude.ToString().Split('.')[0];
                                    HeadingAccuracy = lastlocation.Heading.ToString();
                                    gpsModel.HeadingAccuracy = lastlocation.Heading.ToString().Split('.')[0];
                                    gpsModel.PositionAccuracy = PositionAccuracy = lastlocation.Accuracy.ToString();
                                    gpsModel.Speed = (lastlocation.Speed * 2.237).ToString().Split('.')[0];
                                    Speed = (lastlocation.Speed * 2.237).ToString();
                                    TimeStamp = lastlocation.Timestamp.ToString("MM/d/yyyy hh:mm:ss tt");
                                    gpsModel.TimeStamp = lastlocation.Timestamp.ToString("MMM d hh:mm:ss tt");
                                    Accuracy = lastlocation.Accuracy.ToString();
                                    gpsModel.Accuracy = lastlocation.Accuracy.ToString().Split('.')[0];
                                    gpsModel.Heading = Heading = lastlocation.Heading.ToString().Split('.')[0];
                                    var db = await Utils.Init();
                                    var isInserted = await db.InsertAsync(gpsModel);
                                    if (isInserted > 0) {
                                        MessagingCenter.Send<object, GpsModel>(this, "LogRegistered", gpsModel);
                                        LogCount++;
                                    }

                                    AppStatusText = "Waiting for next reading";
                                }

                            }
                        } else {
                            GpsStatus = "Deactive";
                        }

                    } else {
                        AppStatusText = "Permission Denied";
                    }
                } catch (Exception Exception) {

                }

            });
        }


        //Reading heading value
        private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e) {
            Heading = e.Reading.HeadingMagneticNorth.ToString();

        }



        //GetLocation after N Seconds and register logs
        private void OnTimedEventAsync(object sender, ElapsedEventArgs e) {

            MainThread.BeginInvokeOnMainThread(async () => {
                try {
                    CancellationToken cts;

                    if (Connectivity.NetworkAccess == NetworkAccess.Internet) {
                        AppStatusText = "Connecting to GPS";
                        var locator = CrossGeolocator.Current;
                        //  Location lastlocation = await GetCurrentLocation();
                        cts = new CancellationToken();
                        var lastlocation = await locator.GetPositionAsync(null, cts, true);
                        if (lastlocation != null) {
                            GpsModel gpsModel = new GpsModel();
                            Latitude = lastlocation.Latitude.ToString();
                            gpsModel.Latitude = lastlocation.Latitude.ToString().Split('.')[0];
                            Longitude = lastlocation.Longitude.ToString();
                            gpsModel.Longitude = lastlocation.Longitude.ToString().Split('.')[0];
                            // gpsModel.Heading = Heading = lastlocation.ToString();
                            //gpsModel.HeadingAccuracy = HeadingAccuracy = lastlocation.VerticalAccuracy.ToString();
                            PositionAccuracy = lastlocation.Accuracy.ToString();
                            gpsModel.PositionAccuracy = lastlocation.Accuracy.ToString().Split('.')[0];
                            gpsModel.Speed = (lastlocation.Speed * 2.237).ToString().Split('.')[0];
                            Speed = (lastlocation.Speed * 2.237).ToString().Split('.')[0];
                            TimeStamp = lastlocation.Timestamp.ToString("MM/dd//yyyy hh:mm:ss tt");
                            gpsModel.TimeStamp = lastlocation.Timestamp.ToString("MMM d hh:mm:ss tt");
                            Accuracy = lastlocation.Accuracy.ToString();
                            gpsModel.Accuracy = lastlocation.Accuracy.ToString().Split('.')[0];
                            gpsModel.Heading = Heading.ToString().Split(',')[0];
                            Compass.ReadingChanged += Compass_ReadingChanged;
                            var db = await Utils.Init();
                            var isInserted = await db.InsertAsync(gpsModel);
                            if (isInserted > 0) {
                                MessagingCenter.Send<object, GpsModel>(this, "LogRegistered", gpsModel);
                                LogCount++;
                            }
                            if (IsBusy) {
                                AppStatusText = "GPS Deactivated";
                            } else {
                                AppStatusText = "Waiting for next reading";
                            }

                        }

                    }
                } catch (Exception ex) {

                }

            });

        }

        public void TestLocation(Plugin.Geolocator.Abstractions.Position lastlocation) {
            MainThread.BeginInvokeOnMainThread(async () => {
                try {
                    CancellationToken cts;
                    if (LastTimeStamp != lastlocation.Timestamp.LocalDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) {
                        if (Connectivity.NetworkAccess == NetworkAccess.Internet) {
                            AppStatusText = "Connecting to GPS";
                            if (lastlocation != null) {
                                GpsModel gpsModel = new GpsModel();
                                Latitude = lastlocation.Latitude.ToString();
                                gpsModel.Latitude = lastlocation.Latitude.ToString().Split('.')[0];
                                Longitude = lastlocation.Longitude.ToString();
                                gpsModel.Longitude = lastlocation.Longitude.ToString().Split('.')[0];
                                Heading = lastlocation.Heading.ToString();
                                //gpsModel.HeadingAccuracy = HeadingAccuracy = lastlocation.ac.ToString();
                                PositionAccuracy = lastlocation.Accuracy.ToString();
                                gpsModel.PositionAccuracy = lastlocation.Accuracy.ToString().Split('.')[0];
                                gpsModel.Speed = (lastlocation.Speed * 2.237).ToString().Split('.')[0];
                                Speed = (lastlocation.Speed * 2.237).ToString();
                                TimeStamp = lastlocation.Timestamp.LocalDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                LastTimeStamp = lastlocation.Timestamp.LocalDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                gpsModel.TimeStamp = lastlocation.Timestamp.LocalDateTime.ToString("MMM d hh:mm:ss tt");
                                Accuracy = lastlocation.Accuracy.ToString();
                                gpsModel.Accuracy = lastlocation.Accuracy.ToString().Split('.')[0];
                                gpsModel.Heading = lastlocation.Heading.ToString().Split('.')[0];
                                Compass.ReadingChanged += Compass_ReadingChanged;
                                var db = await Utils.Init();
                                var isInserted = await db.InsertAsync(gpsModel);
                                if (isInserted > 0) {
                                    MessagingCenter.Send<object, GpsModel>(this, "LogRegistered", gpsModel);
                                    LogCount++;
                                }
                                if (IsBusy) {
                                    AppStatusText = "GPS Deactivated";
                                } else {
                                    AppStatusText = "Waiting for next reading";
                                }

                            }

                        }
                    }

                } catch (Exception ex) {

                }

            });
        }
        public void TestLocationAndroid(Xamarin.Essentials.Location lastlocation) {
            MainThread.BeginInvokeOnMainThread(async () => {
                try {
                    CancellationToken cts;
                    if (LastTimeStamp != lastlocation.Timestamp.LocalDateTime.ToString("MM/dd/yyyy hh:mm:ss tt")) {
                        if (Connectivity.NetworkAccess == NetworkAccess.Internet) {
                            AppStatusText = "Connecting to GPS";
                            if (lastlocation != null) {
                                GpsModel gpsModel = new GpsModel();
                                Latitude = lastlocation.Latitude.ToString();
                                gpsModel.Latitude = lastlocation.Latitude.ToString().Split('.')[0];
                                Longitude = lastlocation.Longitude.ToString();
                                gpsModel.Longitude = lastlocation.Longitude.ToString().Split('.')[0];
                                //Heading = lastlocation.Heading.ToString();
                                //gpsModel.HeadingAccuracy = HeadingAccuracy = lastlocation.ac.ToString();
                                PositionAccuracy = lastlocation.Accuracy.ToString();
                                gpsModel.PositionAccuracy = lastlocation.Accuracy.ToString().Split('.')[0];
                                gpsModel.Speed = (lastlocation.Speed * 2.237).ToString().Split('.')[0];
                                Speed = (lastlocation.Speed * 2.237).ToString();
                                TimeStamp = lastlocation.Timestamp.LocalDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                LastTimeStamp = lastlocation.Timestamp.LocalDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                gpsModel.TimeStamp = lastlocation.Timestamp.LocalDateTime.ToString("MMM d hh:mm:ss tt");
                                Accuracy = lastlocation.Accuracy.ToString();
                                gpsModel.Accuracy = lastlocation.Accuracy.ToString().Split('.')[0];
                                Compass.ReadingChanged += Compass_ReadingChanged;
                                gpsModel.Heading = Heading.ToString().Split('.')[0];
                                var db = await Utils.Init();
                                var isInserted = await db.InsertAsync(gpsModel);
                                if (isInserted > 0) {
                                    MessagingCenter.Send<object, GpsModel>(this, "LogRegistered", gpsModel);
                                    LogCount++;
                                }
                                if (IsBusy) {
                                    AppStatusText = "GPS Deactivated";
                                } else {
                                    AppStatusText = "Waiting for next reading";
                                }

                            }

                        }
                    }

                } catch (Exception ex) {

                }

            });
        }
        public void TestLocationAndroidTest(GpsModel lastlocation) {
            MainThread.BeginInvokeOnMainThread(async () => {
                try {
                    CancellationToken cts;
                    if (LastTimeStamp != lastlocation.TimeStamp) {
                        if (Connectivity.NetworkAccess == NetworkAccess.Internet) {
                            AppStatusText = "Connecting to GPS";
                            if (lastlocation != null) {
                                GpsModel gpsModel = new GpsModel();
                                Latitude = lastlocation.Latitude.ToString();
                                gpsModel.Latitude = lastlocation.Latitude.ToString().Split('.')[0];
                                Longitude = lastlocation.Longitude.ToString();
                                gpsModel.Longitude = lastlocation.Longitude.ToString().Split('.')[0];
                                //Heading = lastlocation.Heading.ToString();
                                //gpsModel.HeadingAccuracy = HeadingAccuracy = lastlocation.ac.ToString();
                                // PositionAccuracy = lastlocation.Accuracy.ToString();
                                // gpsModel.PositionAccuracy = lastlocation.Accuracy.ToString().Split('.')[0];
                                // gpsModel.Speed = (lastlocation.Speed * 2.237).ToString().Split('.')[0];
                                // Speed = (lastlocation.Speed * 2.237).ToString();
                                // TimeStamp = lastlocation.Timestamp.LocalDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                // LastTimeStamp = lastlocation.Timestamp.LocalDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                // gpsModel.TimeStamp = lastlocation.Timestamp.LocalDateTime.ToString("MMM d hh:mm:ss tt");
                                //Accuracy = lastlocation.Accuracy.ToString();
                                //gpsModel.Accuracy = lastlocation.Accuracy.ToString().Split('.')[0];
                                //Compass.ReadingChanged += Compass_ReadingChanged;
                                //gpsModel.Heading = Heading.ToString().Split('.')[0];
                                var db = await Utils.Init();
                                var isInserted = await db.InsertAsync(gpsModel);
                                if (isInserted > 0) {
                                    MessagingCenter.Send<object, GpsModel>(this, "LogRegistered", gpsModel);
                                    LogCount++;
                                }
                                if (IsBusy) {
                                    AppStatusText = "GPS Deactivated";
                                } else {
                                    AppStatusText = "Waiting for next reading";
                                }

                            }

                        }
                    }

                } catch (Exception ex) {

                }

            });
        }
        private async Task<Location> GetCurrentLocation() {
            var location = new Location();
            try {

                CancellationTokenSource cts;
                {

                    PermissionStatus permissionStatus = await CheckAndRequestLocationPermission();
                    try {

                        if (permissionStatus == PermissionStatus.Granted) {
                            if (DependencyService.Get<IGpsService>().IsGpsEnable()) {
                                var request = new GeolocationRequest(this.GeolocationAccuracy);
                                cts = new CancellationTokenSource();
                                location = await Geolocation.GetLocationAsync(request, cts.Token);
                            }

                        }
                    } catch (Exception Exception) {

                    }
                }
            } catch (Exception ex) {

            }
            return location;
        }
        //Get count of the registered logs
        public async void GetCount() {
            var db = await Utils.Init();
            LogCount = await db.Table<GpsModel>().CountAsync();
        }
        //Check and request location permission from the user
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

        private void StartService() {
            var startServiceMessage = new StartServiceMessage();
            MessagingCenter.Send(startServiceMessage, "ServiceStarted");
            Preferences.Set("LocationServiceRunning", true);
            //  locationLabel.Text = "Location Service has been started!";
        }

        private void StopService() {
            var stopServiceMessage = new StopServiceMessage();
            MessagingCenter.Send(stopServiceMessage, "ServiceStopped");
            Preferences.Set("LocationServiceRunning", false);
        }
        void HandleReceivedMessages() {
            MessagingCenter.Subscribe<object, Plugin.Geolocator.Abstractions.Position>(this, "Location", (sender, args) => {
                Device.BeginInvokeOnMainThread(() => {
                    var location = args as Plugin.Geolocator.Abstractions.Position;
                    TestLocation(location);
                });
            });
            MessagingCenter.Subscribe<object, Xamarin.Essentials.Location>(this, "LocationAndroid", (sender, args) => {
                Device.BeginInvokeOnMainThread(() => {
                    var location = args as Xamarin.Essentials.Location;
                    TestLocationAndroid(location);
                });

            });
            MessagingCenter.Subscribe<StopServiceMessage>(this, "ServiceStopped", message => {
                Device.BeginInvokeOnMainThread(() => {

                });
            });

            MessagingCenter.Subscribe<object, GpsModel>(this, "LocationAndroidTest", (sender, args) => {
                Device.BeginInvokeOnMainThread(() => {
                    var location = args as GpsModel;
                    TestLocationAndroidTest(location);
                });

            });
            MessagingCenter.Subscribe<LocationErrorMessage>(this, "LocationError", message => {
                Device.BeginInvokeOnMainThread(() => {

                });
            });
        }
        public async Task OnStartClick() {
            await Start();
        }
        public async Task OnStopClick() {
            var message = new StopServiceMessage();
            MessagingCenter.Send(message, "ServiceStopped");
            //UserMessage = "Location Service has been stopped!";
            await SecureStorage.SetAsync(Utils.SERVICE_STATUS_KEY, "0");
            StartEnabled = true;
            StopEnabled = false;
        }

        public async Task ValidateStatus() {
            var status = await SecureStorage.GetAsync(Utils.SERVICE_STATUS_KEY);
            if (status != null && status.Equals("1")) {
                await Start();
            }
        }

        async Task Start() {
            var message = new StartServiceMessage();
            MessagingCenter.Send(message, "ServiceStarted");
            // UserMessage = "Location Service has been started!";
            await SecureStorage.SetAsync(Utils.SERVICE_STATUS_KEY, "1");
            StartEnabled = false;
            StopEnabled = true;
        }
        #region Properties
        bool _isBusy;
        public bool IsBusy {
            get { return _isBusy; }
            set {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    
        #endregion
    }
}
#endregion