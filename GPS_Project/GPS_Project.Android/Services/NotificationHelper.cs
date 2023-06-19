using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using GPS_Project.Droid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[assembly: Xamarin.Forms.Dependency(typeof(NotificationHelper))]
namespace GPS_Project.Droid.Services {
    public class NotificationHelper : INotification {
        private static readonly string foregroundChannelId = "9001";
        private static readonly Context context = global::Android.App.Application.Context;


        public Notification ReturnNotif() {
            var intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            intent.PutExtra("Title", "Message");

            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.Immutable);

            var notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
                .SetContentTitle("Your Title")
                .SetContentText("Your Message")
                
                .SetOngoing(true)
                .SetContentIntent(pendingIntent);

            if (global::Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O) {
                NotificationChannel notificationChannel = new NotificationChannel(foregroundChannelId, "Title", NotificationImportance.High) {
                    Importance = NotificationImportance.High
                };
                notificationChannel.EnableLights(true);
                notificationChannel.EnableVibration(true);
                notificationChannel.SetShowBadge(true);
                notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300 });

                if (context.GetSystemService(Context.NotificationService) is NotificationManager notifManager) {
                    notifBuilder.SetChannelId(foregroundChannelId);
                    notifManager.CreateNotificationChannel(notificationChannel);
                }
            }

            return notifBuilder.Build();
        }
    }
}