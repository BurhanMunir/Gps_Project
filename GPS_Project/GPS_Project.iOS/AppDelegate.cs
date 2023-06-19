using System;
using System.Collections.Generic;
using System.Linq;
using CoreLocation;
using Foundation;
using GPS_Project.iOS.Services;
using GPS_Project.Models;
using UIKit;
using Xamarin.Forms;

namespace GPS_Project.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        // iOsLocationService locationService;
        iOsLocationService locationService;
      //  readonly CLLocationManager locMgr = new CLLocationManager();
       // public static LocationManager Manager = null;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            locationService = new iOsLocationService();
            SetServiceMethods();
            
            global::Xamarin.Forms.Forms.Init();
            
            LoadApplication(new App());

           // UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            //Manager = new LocationManager();
            return base.FinishedLaunching(app, options);
         

            //UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            //Background Location Permissions
           /* if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                locMgr.RequestAlwaysAuthorization();
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                locMgr.AllowsBackgroundLocationUpdates = true;
            }
           */
            return base.FinishedLaunching(app, options);
            
        }

        void SetServiceMethods()
        {
            MessagingCenter.Subscribe<StartServiceMessage>(this, "ServiceStarted", async message => {
                //if (!locationService.isStarted)
                    await locationService.Start();
            });

            MessagingCenter.Subscribe<StopServiceMessage>(this, "ServiceStopped", message => {
               // if (locationService.isStarted)
                    locationService.Stop();
            });
        }

       /* public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            try
            {
                completionHandler(UIBackgroundFetchResult.NewData);
            }
            catch (Exception)
            {
                completionHandler(UIBackgroundFetchResult.NoData);
            }
        }*/

        /*public override void DidEnterBackground(UIApplication uiApplication)
        {
            Manager.StartLocationUpdates();
            base.DidEnterBackground(uiApplication);
           
        }
        public override void OnActivated(UIApplication uiApplication)
        {
            Manager.StopLocationUpdates();
            base.OnActivated(uiApplication);
        }*/

    }
}
