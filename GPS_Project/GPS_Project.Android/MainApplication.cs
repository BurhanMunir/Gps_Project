using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Shiny;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPS_Project.Droid {
    [Application]
    public class MainApplication :Application{
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

        public override void OnCreate() {
            this.ShinyOnCreate(new MyStartup());
            base.OnCreate();
        }
    }
}