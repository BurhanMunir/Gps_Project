using GPS_Project.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GPS_Project
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage =new NavigationPage(new SetupView());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        
    }
}
