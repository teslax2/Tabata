using Android.App;
using Android.Content.Res;
using Android.OS;
using Tabata.ViewModel;
using Xamarin.Forms;
using TaskStackBuilder = Android.Support.V4.App;
using Android.Content;
using System;
using Android.Support.V4.App;

[assembly: Dependency(typeof(Tabata.Droid.Notification))]
namespace Tabata.Droid
{
    public class Notification : INotification
    {
        private static readonly string channelName = "Local Notifications";
        private static readonly string description = "Nananan";
        private static readonly string channelId = "location_notification";
        private static readonly int notificationId = 1000;
        private int count = 0;
        private NotificationCompat.Builder builder;
        private NotificationManagerCompat notificationManager;

        public Notification()
        {
            CreateNotificationChannel();
        }

        public void Create(string message)
        {
            // Set up an intent so that tapping the notifications returns to this app:
            Intent intent = MainActivity.Instance.Intent;

            // Create a PendingIntent; we're only using one PendingIntent (ID = 0):
            const int pendingIntentId = 0;
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(MainActivity.Instance.ApplicationContext, pendingIntentId, intent, PendingIntentFlags.OneShot);

            // Build the notification:
            builder = new NotificationCompat.Builder(Android.App.Application.Context, channelId)
                          .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                          .SetContentIntent(pendingIntent) // Start up this activity when the user clicks the intent.
                          .SetContentTitle("Tabata is running!") // Set the title
                          .SetNumber(count) // Display the count in the Content Info
                          .SetSmallIcon(Resource.Drawable.tabata) // This is the icon to display
                          .SetContentText(message); // the message to display.
            

            // Finally, publish the notification:
            notificationManager = NotificationManagerCompat.From(Android.App.Application.Context);
            notificationManager.Notify(notificationId, builder.Build());
        }

        public void Hide()
        {
            notificationManager.CancelAll();
        }

        public void Update(string message)
        {
            if (builder == null)
                return;
            builder.SetContentText(message);
            notificationManager.Notify(notificationId, builder.Build());
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default)
            {
                Description = description
            };

            var notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}