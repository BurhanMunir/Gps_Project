using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GPS_Project.Droid;
using GPS_Project.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
[assembly: Xamarin.Forms.Dependency(typeof(LocationConsent))]
namespace GPS_Project.Droid {
    public class LocationConsent : ILocationConsent {
        public async Task GetLocationConsent() {
            await Permissions.RequestAsync<Permissions.LocationAlways>();
        }
    }
}