using GPS_Project.ViewModels;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GPS_Project.Interfaces;

namespace GPS_Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupView : ContentPage
    {
        MainViewModel viewmodel;
        readonly ILocationConsent locationConsent;
        public SetupView()
        {
            InitializeComponent();
            locationConsent = DependencyService.Get<ILocationConsent>();
            viewmodel = new MainViewModel(Navigation);
            BindingContext=viewmodel;
           // CheckPermission();
        }
        protected async override void OnAppearing() {
            base.OnAppearing();
            //await locationConsent.GetLocationConsent();
            //await viewmodel.ValidateStatus();
        }
        //public async Task<PermissionStatus> CheckAndRequestLocationPermission() {
        //    var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
        //    try {

        //        if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
        //            return status;

        //        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS) {
        //            // Prompt the user to turn on in settings
        //            // On iOS once a permission has been denied it may not be requested again from the application
        //            return status;
        //        }

        //        if (Permissions.ShouldShowRationale<Permissions.LocationAlways>()) {
        //            // Prompt the user with additional information as to why the permission is needed
        //        }

        //        status = await Permissions.RequestAsync<Permissions.LocationAlways>();

        //    } catch (Exception ex) {

        //    }
        //    return status;
        //}

        public async Task CheckPermission() {
            try {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationAlwaysPermission>();
                if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted) {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.LocationAlways)) {
                        await DisplayAlert("Need location", "Gunna need that location", "OK");
                    }

                    status = await CrossPermissions.Current.RequestPermissionAsync<LocationAlwaysPermission>();
                }

                if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted) {
                    //Query permission
                } else if (status != Plugin.Permissions.Abstractions.PermissionStatus.Unknown) {
                    //location denied
                }
            } catch (Exception ex) {
                //Something went wrong
            }
        }
    }
}