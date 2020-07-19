using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AmbuTap_Driver.Helpers
{
    class LocationCallbackHelper : LocationCallback  // inherits from location callback
    {
        //  create an event handler that will be called in Main Activity
        public EventHandler<OnLocationCapturedEventArgs> MyLocation;

        public class OnLocationCapturedEventArgs : EventArgs
        {
            public Android.Locations.Location Location { get; set; }
        }


        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            // check if location is available
            Log.Debug("AmbuTap", "IsLocationAvailable: {0}", locationAvailability.IsLocationAvailable);
        }

        public override void OnLocationResult(LocationResult result)
        {
            // once current location is captured in 'MyLocation' above, send it here
            if (result.Locations.Count != 0)
            {
                // location call back
                MyLocation?.Invoke(this, new OnLocationCapturedEventArgs { Location = result.Locations[0] });
            }
        } 
    }
}