using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AmbuTap_Driver.Helpers
{
    public class NotificationHelper : Java.Lang.Object
    {
        public const string PRIMARY_CHANNEL = "Urgent";
        public const int NOTIFY_ID = 100;

        public void NotifyVersion26(Context context, Android.Content.Res.Resources res, Android.App.NotificationManager manager)
        {
            string channelName = "Secondary Channel";
            var importance = NotificationImportance.High;
            var channel = new NotificationChannel(PRIMARY_CHANNEL, channelName, importance);

            var path = Android.Net.Uri.Parse("android.resource://com.companyname.ambutap_driver/" + Resource.Raw.alert);
            var audioattribute = new AudioAttributes.Builder()
                .SetContentType(AudioContentType.Sonification)
                .SetUsage(AudioUsageKind.Notification).Build();

            channel.EnableLights(true);
            channel.EnableVibration(true);
            channel.SetSound(path, audioattribute);
            channel.LockscreenVisibility = NotificationVisibility.Public;

            manager.CreateNotificationChannel(channel);

            Intent intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);

            Notification.Builder builder = new Notification.Builder(context)
                .SetContentTitle("AmbuTap Driver")
                .SetSmallIcon(Resource.Drawable.ic_location)
                .SetLargeIcon(BitmapFactory.DecodeResource(res, Resource.Drawable.ambutaplogo))
                .SetContentText("You have a new request")
                .SetChannelId(PRIMARY_CHANNEL)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            manager.Notify(NOTIFY_ID, builder.Build());
        }
    }
}