using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmbuTap_Driver.Helpers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Java.Util;

namespace AmbuTap_Driver.EventListener
{
    class AvailabilityListener : Java.Lang.Object, IValueEventListener
    {
        FirebaseDatabase database;
        DatabaseReference availabilityRef;

        // Events
        public class RideAssignedIDEventArgs : EventArgs
        {
            public string RideId { get; set; }
        }
        public event EventHandler<RideAssignedIDEventArgs> RideAssigned;
        public event EventHandler RideCancelled;
        public event EventHandler RideTimedOut;


        public void OnCancelled(DatabaseError error)
        {
            
        }


        // Receiving request on new ride
        // called in Main activity - TakeDriverOnline method
        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                string ride_id = snapshot.Child("ride_id").Value.ToString();
                if (ride_id != "waiting" && ride_id != "TimeOut" && ride_id != "Cancelled") 
                {
                    // Ride Assigned
                    RideAssigned?.Invoke(this, new RideAssignedIDEventArgs { RideId = ride_id }); 
                }
                else if (ride_id == "TimeOut") 
                {
                    // Ride Timeout
                    RideTimedOut?.Invoke(this, new EventArgs());
                }
                else if (ride_id == "Cancelled")
                {
                    // Ride Cancelled
                    RideCancelled?.Invoke(this, new EventArgs());  
                }

            }
        }


        // go online method
        public void Create (Android.Locations.Location myLocation)
        {
            database = AppDataHelper.GetDatabase();
            string driverID = AppDataHelper.GetCurrentUser().Uid;

            availabilityRef = database.GetReference("driversAvailable/" + driverID);

            // hash map to store the location details
            HashMap location = new HashMap();
            location.Put("latitude", myLocation.Latitude);
            location.Put("longitude", myLocation.Longitude);


            // hash map to store the driverinfo details
            HashMap driverInfo = new HashMap();
            driverInfo.Put("location", myLocation);
            driverInfo.Put("ride_id", "waiting");

            availabilityRef.AddValueEventListener(this);
            availabilityRef.SetValue(driverInfo);

        }

        // take the driver offline
        public void RemoveListener()
        {
            availabilityRef.RemoveValue();
            availabilityRef.RemoveEventListener(this);
            availabilityRef = null;
        }
        

        // update driver location in firebase
        public void UpdateLocation(Android.Locations.Location myLocation)
        {
            string DriverId = AppDataHelper.GetCurrentUser().Uid;

            if(availabilityRef != null)
            {
                DatabaseReference locationref = database.GetReference("driversAvailable/" + DriverId + "/location");
                HashMap locationMap = new HashMap();
                locationMap.Put("latitude", myLocation.Latitude);
                locationMap.Put("longitude", myLocation.Longitude);
                locationref.SetValue(locationMap);
            }
        }


        // to ensure driver returns online when timed out and when cancelled
        // called in Main activity - TakeDriverOnline method
        public void ReActivate()
        {
            availabilityRef.Child("ride_id").SetValue("waiting");
        }
    }
}